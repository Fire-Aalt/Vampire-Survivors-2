using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class DenoiserAlgo
    {
        public static TilesData Denoise(in ChunkRequestData requestData, NoiseToTilesSettings settings)
        {
            sbyte[,] layersMatrix = requestData.layersMatrix.matrix;
            int tilesInLine = layersMatrix.GetLength(0);

            sbyte[,] neighboursCount = new sbyte[tilesInLine, tilesInLine];
            Queue<Vector2Int> tilesToRemove = new();

            for (int it = 0; it < 10; it++)
            {
                for (int tileY = 0; tileY < tilesInLine; tileY++)
                {
                    for (int tileX = 0; tileX < tilesInLine; tileX++)
                    {
                        sbyte neighbours = 0;
                        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.up);
                        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.right);
                        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.down);
                        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.left);

                        neighboursCount[tileX, tileY] = neighbours;
                        if (neighbours < 2 && layersMatrix[tileX, tileY] > -1)
                        {
                            //tilesToRemove.Enqueue(new Vector2Int(tileX, tileY));
                            layersMatrix[tileX, tileY]--;
                            //DecreaseNeigboursCount(tilesInLine, neighboursCount, tilesToRemove, tileX, tileY);
                        }
                    }
                }

                //while (tilesToRemove.Count > 0)
                //{
                //    Vector2Int coord = tilesToRemove.Dequeue();

                //    layersMatrix[coord.x, coord.y]--;
                //    DecreaseNeigboursCount(layersMatrix, neighboursCount, tilesInLine, tilesToRemove, coord.x, coord.y);
                //}
            }

            
            //for (int tileY = 0; tileY < tilesInLine; tileY++)
            //{
            //    for (int tileX = 0; tileX < tilesInLine; tileX++)
            //    {
            //        sbyte neighbours = 0;
            //        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.up);
            //        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.right);
            //        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.down);
            //        neighbours += GetNeighbour(requestData, tilesInLine, tileX, tileY, Vector2Int.left);

            //        if (neighbours < 2 && layersMatrix[tileX, tileY] > -1)
            //        {
            //            layersMatrix[tileX, tileY]--;
            //        }
            //    }
            //}

            Color32[,] colorMap = GenerateTilesColorMap(requestData, settings);
            return new TilesData(layersMatrix, colorMap);
        }

        private static void DecreaseNeigboursCount(sbyte[,] layersMatrix, sbyte[,] neighboursCount, int tilesInLine, Queue<Vector2Int> tilesToRemove, int tileY, int tileX)
        {
            DecreaseNeighbours(layersMatrix, neighboursCount, tilesToRemove, tilesInLine, tileX, tileY + 1);
            DecreaseNeighbours(layersMatrix, neighboursCount, tilesToRemove, tilesInLine, tileX + 1, tileY);
            DecreaseNeighbours(layersMatrix, neighboursCount, tilesToRemove, tilesInLine, tileX, tileY - 1);
            DecreaseNeighbours(layersMatrix, neighboursCount, tilesToRemove, tilesInLine, tileX - 1, tileY);
        }

        private static sbyte GetNeighbour(in ChunkRequestData requestData, int tilesInLine, int x, int y, Vector2Int offset)
        {
            sbyte[,] layersMatrix = requestData.layersMatrix.matrix;
            ActiveChunk[,] chunksMatrix = requestData.chunksMatrix;
            Vector2Int center = requestData.chunksMatrixCenter;

            Vector2Int pos = new(x + offset.x, y + offset.y);

            sbyte layer;
            if (pos.x < 0)
            {
                layer = chunksMatrix[center.x - 1, center.y].RawLayersMatrix.matrix[tilesInLine + pos.x, y];
            }
            else if (pos.x >= tilesInLine)
            {
                layer = chunksMatrix[center.x + 1, center.y].RawLayersMatrix.matrix[pos.x - tilesInLine, y];
            }
            else if (pos.y < 0)
            {
                layer = chunksMatrix[center.x, center.y - 1].RawLayersMatrix.matrix[x, tilesInLine + pos.y];
            }
            else if (pos.y >= tilesInLine)
            {
                layer = chunksMatrix[center.x, center.y + 1].RawLayersMatrix.matrix[x, pos.y - tilesInLine];
            }
            else
            {
                layer = layersMatrix[pos.x, pos.y];
            }

            if (layer >= layersMatrix[x, y])
            {
                return 1;
            }

            return 0;
        }

        private static void DecreaseNeighbours(sbyte[,] layersMatrix, sbyte[,] neighboursCount, Queue<Vector2Int> coordsToCheck, int tilesInLine, int x, int y)
        {
            if (x >= 0 && x < tilesInLine && y >= 0 && y < tilesInLine && layersMatrix[x, y] > -1)
            {                
                neighboursCount[x, y]--;
                int n = neighboursCount[x, y];

                if (n < 2)
                {
                    coordsToCheck.Enqueue(new Vector2Int(x, y));
                }
            }
        }

        private static Color32[,] GenerateTilesColorMap(in ChunkRequestData requestData, NoiseToTilesSettings settings)
        {
            sbyte[,] layersMatrix = requestData.layersMatrix.matrix;
            int chunkSize = WorldGenerator.chunkSizeInTiles;

            Color32[,] tilesColorMap = new Color32[chunkSize, chunkSize];
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    sbyte currentLayer = layersMatrix[x, y];
                    if (currentLayer <= -1)
                    {
                        tilesColorMap[x, y] = Color.clear;
                    }
                    else
                    {
                        tilesColorMap[x, y] = settings.tileLayers[currentLayer].layerColor;
                    }
                }
            }
            return tilesColorMap;
        }
    }
}
