using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ChunkDataGenerator
    {
        public NoiseDataSO noiseData;
        public TerrainDataSO terrainData;
        public ObjectPlacementDataSO objectPlacementData;
        public int seed;

        private readonly int _chunkSizeInTiles;

        public ChunkDataGenerator(WorldGenerationDataSO generationData, int seed)
        {
            noiseData = generationData.worldLayersNoiseData;
            terrainData = generationData.terrainData;
            objectPlacementData = generationData.objectPlacementData;
            this.seed = seed;

            _chunkSizeInTiles = WorldGenerator.chunkSizeInTiles;
        }

        public LayersMatrix GenerateRawData(Vector2 center)
        {
            float[,] noiseMap = Noise.GenerateLayeredNoiseMap(_chunkSizeInTiles, noiseData.noiseLayers, seed, center, Noise.NormalizeMode.Global);
            return NoiseToLayers.GenerateLayersMatrix(noiseMap, terrainData.noiseToTilesSettings);
        }

        public ChunkData GenerateCompleteData(in ChunkRequestData requestData)
        {
            MapData mapData = GenerateMapData(requestData.chunkCenter, Noise.NormalizeMode.Global);
            TilesData tilesData = GenerateTilesData(requestData);
            ObjectsData objectsData = GenerateObjectsData(requestData);

            return new ChunkData(mapData, tilesData, objectsData);
        }

        /// <summary>
        /// Depricated. Does not account for any changes to layersMatrix
        /// </summary>
        /// <param name="center"></param>
        /// <param name="normalizeMode"></param>
        /// <returns></returns>
        private MapData GenerateMapData(Vector2 center, Noise.NormalizeMode normalizeMode)
        {
            float[,] noiseMap = Noise.GenerateLayeredNoiseMap(_chunkSizeInTiles, noiseData.noiseLayers, seed, center, normalizeMode);

            Color32[,] colorMap = new Color32[_chunkSizeInTiles, _chunkSizeInTiles];
            for (int y = 0; y < _chunkSizeInTiles; y++)
            {
                for (int x = 0; x < _chunkSizeInTiles; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < terrainData.terrainTypes.Count; i++)
                    {
                        if (currentHeight >= terrainData.terrainTypes[i].startHeight)
                        {
                            colorMap[x, y] = terrainData.terrainTypes[i].color;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap, _chunkSizeInTiles, _chunkSizeInTiles);
        }

        private TilesData GenerateTilesData(in ChunkRequestData requestData)
        {
            return DenoiserAlgo.Denoise(requestData, terrainData.noiseToTilesSettings);
        }

        private ObjectsData GenerateObjectsData(in ChunkRequestData requestData)
        {
            int layersCount = objectPlacementData.layers.Count;
            List<float[,]> layersHeightMaps = new();

            for (int i = 0; i < layersCount; i++)
            {
                ObjectPlacementLayer placementLayer = objectPlacementData.layers[i];

                if (placementLayer.noiseType == NoiseType.Single)
                {
                    layersHeightMaps.Add(Noise.GenerateNoiseMap(_chunkSizeInTiles, placementLayer.singleNoise, seed, requestData.chunkCenter, Noise.NormalizeMode.Global));
                }
                else if (placementLayer.noiseType == NoiseType.Layered)
                {
                    layersHeightMaps.Add(Noise.GenerateLayeredNoiseMap(_chunkSizeInTiles, placementLayer.layeredNoise.noiseLayers, seed, requestData.chunkCenter, Noise.NormalizeMode.Global));
                }
            }

            return new ObjectsData(layersHeightMaps);
        }
    }

    public readonly struct ChunkRequestData
    {
        public readonly LayersMatrix layersMatrix;
        public readonly ActiveChunk[,] chunksMatrix;
        public readonly Vector2 chunkCenter;
        public readonly Vector2Int chunksMatrixCenter;

        public ChunkRequestData(in LayersMatrix layersMatrix, ActiveChunk[,] chunksMatrix, Vector2 chunkCenter)
        {
            this.layersMatrix = layersMatrix;
            this.chunksMatrix = chunksMatrix;
            this.chunkCenter = chunkCenter;
            chunksMatrixCenter = new Vector2Int(chunksMatrix.GetLength(0) / 2, chunksMatrix.GetLength(0) / 2);
        }
    }

    public readonly struct ChunkData
    {
        public readonly MapData mapData;
        public readonly TilesData tilesData;
        public readonly ObjectsData objectsData;

        public ChunkData(in MapData mapData, in TilesData tilesData, in ObjectsData objectsData)
        {
            this.mapData = mapData;
            this.tilesData = tilesData;
            this.objectsData = objectsData;
        }
    }

    public readonly struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color32[,] colorMap;
        public readonly int mapSizeInPixels;
        public readonly int mapSizeInTiles;

        public MapData(float[,] heightMap, Color32[,] colorMap, int mapSizeInPixels, int mapSizeInTiles)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
            this.mapSizeInPixels = mapSizeInPixels;
            this.mapSizeInTiles = mapSizeInTiles;
        }
    }

    public readonly struct TilesData
    {
        public readonly sbyte[,] tilesMatrix;
        public readonly Color32[,] tilesColorMap;

        public TilesData(sbyte[,] tilesMatrix, Color32[,] tilesColorMap)
        {
            this.tilesMatrix = tilesMatrix;
            this.tilesColorMap = tilesColorMap;
        }
    }

    public readonly struct ObjectsData
    {
        public readonly List<float[,]> layersHeightMaps;

        public ObjectsData(List<float[,]> layersHeightMaps)
        {
            this.layersHeightMaps = layersHeightMaps;
        }
    }
}
