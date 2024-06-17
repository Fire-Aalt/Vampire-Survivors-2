using Lean.Pool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class EndlessTerrain : MonoBehaviour
    {
        [Title("Target")]
        [SerializeField] private Transform _viewer;
        [SerializeField, MinValue(0)] private int _chunksVisibleInView;
        [SerializeField, MinValue(0f)] private float _viewerMoveThreshold;
        [SerializeField, MinValue(0)] private int _maxCachedChunks;

        [field: SerializeField, Title("References")] public WorldGenerator WorldGenerator { get; private set; }
        [field: SerializeField] public Chunk ChunkPrefab { get; private set; }
        [field: SerializeField] public Transform ChunksHolder { get; private set; }

        public static Vector2 ViewerPosition { get; private set; }
        public int ChunksVisibleInViewDst { get; private set; }

        private int chunkSize;
        private float viewerMoveThresholdForChunkUpdateSqr;
        private Vector2 viewerPositionOld;

        private readonly Dictionary<Vector2Int, TerrainChunk> _loadedChunksDict = new();
        private static readonly List<TerrainChunk> _loadedChunksList = new();
        public static readonly List<TerrainChunk> visibleTerrainChunks = new();

        private void Start()
        {
            chunkSize = WorldGenerator.chunkSizeInTiles;
            ChunksVisibleInViewDst = _chunksVisibleInView * chunkSize;
            viewerMoveThresholdForChunkUpdateSqr = _viewerMoveThreshold * _viewerMoveThreshold;

            ActiveChunk.InitializeChunks();
            WorldGenerator.InitializeRuntimeGeneration();
            UpdateVisibleChunks();
            UpdateVisibleChunks();
        }

        private void Update()
        {
            ViewerPosition = _viewer.position;
            if ((viewerPositionOld - ViewerPosition).sqrMagnitude > viewerMoveThresholdForChunkUpdateSqr)
            {
                viewerPositionOld = ViewerPosition;
                UpdateVisibleChunks();
            }
        }

        public void UpdateVisibleChunks()
        {
            HashSet<Vector2Int> alreadyUpdatedChunkCoords = new();
            for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
                visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / chunkSize);

            for (int yOffset = -_chunksVisibleInView; yOffset <= _chunksVisibleInView; yOffset++)
            {
                for (int xOffset = -_chunksVisibleInView; xOffset <= _chunksVisibleInView; xOffset++)
                {
                    Vector2Int viewedChunkCoord = new(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (_loadedChunksDict.ContainsKey(viewedChunkCoord))
                        {
                            _loadedChunksDict[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            var chunk = new TerrainChunk(viewedChunkCoord, this, ChunkDrawMode.ColorMap);
                            _loadedChunksDict.Add(viewedChunkCoord, chunk);
                            _loadedChunksList.Add(chunk);

                            if (_loadedChunksList.Count == _maxCachedChunks)
                            {
                                var chunkToRemove = _loadedChunksList.First(c => !c.IsVisible());
                                _loadedChunksDict.Remove(chunkToRemove.coord);
                                _loadedChunksList.Remove(chunkToRemove);

                                chunkToRemove.Dispose();
                            }
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _loadedChunksList.Clear();
            visibleTerrainChunks.Clear();
            ActiveChunk.LoadedChunks.Clear();
        }
    }

    public class TerrainChunk : ActiveChunk
    {
        public override bool UseChunkPool => true;
        private readonly EndlessTerrain _endlessTerrain;

        private Bounds _bounds;

        public TerrainChunk(Vector2Int coord, EndlessTerrain endlessTerrain, ChunkDrawMode drawMode)
            : base(coord, drawMode, endlessTerrain.WorldGenerator, endlessTerrain.ChunkPrefab, endlessTerrain.ChunksHolder)
        {
            _endlessTerrain = endlessTerrain;
            _bounds = new Bounds(position, new Vector2(WorldGenerator.chunkSizeInTiles, WorldGenerator.chunkSizeInTiles));

            SetVisible(false);
            generator.ThreadedDataRequester.RequestLayersMatrix(ChunkCenter, OnLayersMatrixReceived);
        }

        public bool UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(EndlessTerrain.ViewerPosition));
            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= _endlessTerrain.ChunksVisibleInViewDst;

            if (wasVisible != visible)
            {
                if (visible)
                    EndlessTerrain.visibleTerrainChunks.Add(this);
                else
                    EndlessTerrain.visibleTerrainChunks.Remove(this);
                SetVisible(visible);
            }
            return visible;
        }

        private void SetVisible(bool visible)
        {
            chunkObject.SetActive(visible);

            if (ChunkLOD >= ChunkLOD.Colors)
            {
                if (visible)
                    WorldGenerator.Instance.GenerateChunk(coord, Data);
                else
                    WorldGenerator.Instance.DestroyChunk(coord, Data);
            }
        }

        protected override void OnChunkDataReceived(ChunkData data)
        {
            base.OnChunkDataReceived(data);

            bool wasVisible = IsVisible();
            bool visible = UpdateTerrainChunk();
            if (wasVisible && visible)
            {
                SetVisible(true);
            }
        }

        public bool IsVisible() => chunkObject.activeSelf;
    }
}
