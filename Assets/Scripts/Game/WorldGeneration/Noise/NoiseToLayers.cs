using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class NoiseToLayers
    {
        public static LayersMatrix GenerateLayersMatrix(float[,] heightMap, NoiseToTilesSettings settings)
        {
            int tilesInLine = heightMap.GetLength(0);
            sbyte[,] layersMatrix = new sbyte[tilesInLine, tilesInLine];

            for (int tileY = 0; tileY < tilesInLine; tileY++)
            {
                for (int tileX = 0; tileX < tilesInLine; tileX++)
                {
                    float height = heightMap[tileX, tileY];

                    sbyte layer = -1;
                    for (sbyte i = 0; i < settings.tileLayers.Count; i++)
                    {
                        if (height > settings.tileLayers[i].averageHeight)
                        {
                            layer = i;
                        }
                    }

                    layersMatrix[tileX, tileY] = layer;
                }
            }

            return new LayersMatrix(layersMatrix);
        }
    }

    public readonly struct LayersMatrix
    {
        public readonly sbyte[,] matrix;
        
        public LayersMatrix(sbyte[,] matrix)
        {
            this.matrix = matrix;
        }
    }

    [System.Serializable]
    public class NoiseToTilesSettings
    {
        public List<TileLayerType> tileLayers;
    }

    [System.Serializable]
    public struct TileLayerType
    {
        public float averageHeight;
        public Color layerColor;
    }
}
