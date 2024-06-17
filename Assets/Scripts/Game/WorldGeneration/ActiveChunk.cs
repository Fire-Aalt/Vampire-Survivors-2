using Lean.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ActiveChunk : IDisposable
    {
        public static event Action<ActiveChunk> OnChunkLoaded;

        public static Dictionary<Vector2Int, ActiveChunk> LoadedChunks;
        public static readonly int NeighboursMatrixSize = 3;
        public static readonly int NeighboursMatrixSizeHalf = NeighboursMatrixSize / 2;
        public static Vector2Int MatrixCenter;

        public event Action<ChunkLOD> OnChunkLODChanged;
        public abstract bool UseChunkPool { get; }

        public readonly Vector2Int coord;
        public readonly Vector2 position;
        public readonly WorldGenerator generator;
        public readonly Chunk chunk;
        public readonly GameObject chunkObject;
        public readonly bool useThreads;

        public ActiveChunk[,] NeighboursMatrix => chunk.neighboursMatrix;

        public Vector2 ChunkCenter => coord * WorldGenerator.chunkSizeInTiles;

        public ChunkLOD ChunkLOD { get => _chunkLOD; private set 
            {
                _chunkLOD = value;
                OnChunkLODChanged?.Invoke(_chunkLOD);
            }
        }
        protected ChunkLOD _chunkLOD;

        public ChunkDrawMode DrawMode { get; protected set; }
        public ChunkData Data { get; protected set; }
        public LayersMatrix RawLayersMatrix { get; protected set; }
        public bool IsReadyToBeGenerated { get; protected set; }
        protected bool chunkDataRequested;

        public ActiveChunk(Vector2Int coord, ChunkDrawMode drawMode, WorldGenerator generator, Chunk prefab, Transform holder, bool useThreads = true)
        {
            this.coord = coord;
            this.generator = generator;
            this.useThreads = useThreads;
            DrawMode = drawMode;
            position = coord * WorldGenerator.chunkSizeInTiles;

            if (UseChunkPool)
                chunk = LeanPool.Spawn(prefab);
            else 
                chunk = UnityEngine.Object.Instantiate(prefab);

            chunkObject = chunk.gameObject;
            chunk.Initialize(this);

            chunkObject.name = $"{coord.x}x{coord.y} Chunk";
            chunkObject.transform.position = position;
            chunkObject.transform.parent = holder;

            GetAlreadyLoadedChunks();

            ChunkLOD = ChunkLOD.Empty;
            OnChunkLoaded += HandleChunkLoaded;
            OnChunkLoaded?.Invoke(this);
        }

        public static void InitializeChunks()
        {
            LoadedChunks = new Dictionary<Vector2Int, ActiveChunk>();
            MatrixCenter = new(NeighboursMatrixSizeHalf, NeighboursMatrixSizeHalf);
        }

        private void GetAlreadyLoadedChunks()
        {
            LoadedChunks.Add(coord, this);
            for (int y = -NeighboursMatrixSizeHalf; y <= NeighboursMatrixSizeHalf; y++)
            {
                for (int x = -NeighboursMatrixSizeHalf; x <= NeighboursMatrixSizeHalf; x++)
                {
                    Vector2Int offset = new(x, y);
                    Vector2Int lookUpChunkCoord = coord + offset;
                    if (LoadedChunks.ContainsKey(lookUpChunkCoord))
                    {
                        ActiveChunk chunk = LoadedChunks[lookUpChunkCoord];
                        Vector2Int matrixCoord = MatrixCenter + offset;
                        NeighboursMatrix[matrixCoord.x, matrixCoord.y] = chunk;
                    }
                }
            }
        }

        public virtual void RedrawChunk(ChunkDrawMode mode)
        {
            DrawMode = mode;
            if (ChunkLOD >= ChunkLOD.Colors)
            {
                DrawChunk();
            }
        }

        protected virtual void DrawChunk()
        {
            switch (DrawMode)
            {
                case ChunkDrawMode.NoiseMap:
                    chunk.DrawTexture(TextureGenerator.TextureFromHeightMap(Data.mapData.heightMap, WorldGenerator.chunkSizeInTiles));
                    break;
                case ChunkDrawMode.TilesMap:
                    chunk.DrawTexture(TextureGenerator.TextureFromColorMap(Data.tilesData.tilesColorMap, WorldGenerator.chunkSizeInTiles));
                    break;
                case ChunkDrawMode.ColorMap:
                    chunk.DrawTexture(TextureGenerator.TextureFromColorMap(Data.mapData.colorMap, WorldGenerator.chunkSizeInTiles));
                    break;
            }
        }

        protected virtual void HandleChunkLoaded(ActiveChunk chunk)
        {
            Vector2Int pos = MatrixCenter + (chunk.coord - coord);

            if (pos.x >= 0 && pos.x < NeighboursMatrixSize && pos.y >= 0 && pos.y < NeighboursMatrixSize)
            {
                NeighboursMatrix[pos.x, pos.y] = chunk;
                chunk.OnChunkLODChanged += HandleChunkLODChanged;

                int loaded = 0;
                for (int y = 0; y < NeighboursMatrixSize; y++)
                {
                    for (int x = 0; x < NeighboursMatrixSize; x++)
                    {
                        loaded += NeighboursMatrix[x, y] != null ? 1 : 0;
                    }
                }

                if (loaded == NeighboursMatrixSize * NeighboursMatrixSize)
                {
                    OnChunkLoaded -= HandleChunkLoaded;
                }
            }
        }

        protected virtual void HandleChunkLODChanged(ChunkLOD lod)
        {
            if (!chunkDataRequested && !IsReadyToBeGenerated && CompareClosestMatrixChunkLODs(ChunkLOD.Layers))
            {
                if (ChunkLOD == ChunkLOD.Layers)
                {
                    RequestChunkData();
                }
                else
                {
                    IsReadyToBeGenerated = true;
                }
            }

            if (ChunkLOD == ChunkLOD.Colors && CompareAllMatrixChunkLODs(ChunkLOD.Colors))
            {
                generator.GenerateObjects(chunk, Data);
                ChunkLOD = ChunkLOD.Complete;
            }
        }

        protected void RequestChunkData()
        {
            chunkDataRequested = true;
            ChunkRequestData requestData = new(RawLayersMatrix, NeighboursMatrix, ChunkCenter);
            if (useThreads)
                generator.ThreadedDataRequester.RequestChunkData(requestData, OnChunkDataReceived);
            else
                OnChunkDataReceived(generator.DataGenerator.GenerateCompleteData(requestData));
        }

        protected virtual void OnLayersMatrixReceived(LayersMatrix layersMatrix)
        {
            RawLayersMatrix = layersMatrix;
            if (IsReadyToBeGenerated)
            {
                RequestChunkData();
            }

            ChunkLOD = ChunkLOD.Layers;
        }

        protected virtual void OnChunkDataReceived(ChunkData data)
        {
            Data = data;
            DrawChunk();

            ChunkLOD = ChunkLOD.Colors;
        }

        protected bool CompareClosestMatrixChunkLODs(ChunkLOD lod)
        {
            return CompareChunkLOD(Vector2Int.left, lod) && CompareChunkLOD(Vector2Int.right, lod)
                && CompareChunkLOD(Vector2Int.up, lod) && CompareChunkLOD(Vector2Int.down, lod);
        }

        protected bool CompareAllMatrixChunkLODs(ChunkLOD lod)
        {
            return CompareClosestMatrixChunkLODs(lod) &&
                CompareChunkLOD(new Vector2Int(-1, 1), lod) && CompareChunkLOD(new Vector2Int(1, -1), lod)
                && CompareChunkLOD(new Vector2Int(-1, -1), lod) && CompareChunkLOD(new Vector2Int(1, 1), lod);
        }

        protected bool CompareChunkLOD(Vector2Int relativeOffset, ChunkLOD lod)
        {
            Vector2Int coord = MatrixCenter + relativeOffset;
            ActiveChunk chunk = NeighboursMatrix[coord.x, coord.y];
            if (chunk != null)
            {
                return chunk.ChunkLOD >= lod;
            }
            return false;
        }

        public void Dispose()
        {
            if (UseChunkPool)
            {
                LeanPool.Despawn(chunkObject);
            }
            else
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(chunkObject);
                else
                    UnityEngine.Object.DestroyImmediate(chunkObject);
            }

            LoadedChunks.Remove(coord);
            OnChunkLoaded -= HandleChunkLoaded;
        }
    }

    public enum ChunkLOD
    {
        Empty,
        Layers,
        Colors,
        Complete
    }
}
