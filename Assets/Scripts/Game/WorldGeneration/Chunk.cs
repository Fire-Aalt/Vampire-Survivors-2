using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Chunk : MonoBehaviour
    {
        public TerrainDisplay terrainDisplay;
        public Transform objectsHolder;

        [ReadOnly] public Vector2Int coord;
        [HideInInspector] public ActiveChunk[,] neighboursMatrix;
        [HideInInspector] public Vector2Int matrixCenter;
        [HideInInspector] public int cellsInLine;

        public void Initialize(ActiveChunk activeChunk)
        {
            this.coord = activeChunk.coord;
            neighboursMatrix = new ActiveChunk[ActiveChunk.NeighboursMatrixSize, ActiveChunk.NeighboursMatrixSize];
            matrixCenter = ActiveChunk.MatrixCenter;
            cellsInLine = WorldGenerator.chunkSizeInTiles;

            terrainDisplay.spriteRender.enabled = false;
            terrainDisplay.InstantiateMaterial();
            ClearChunk();
        }

        public void ClearChunk()
        {
            for (int i = objectsHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(objectsHolder.GetChild(i).gameObject);
            }
        }

        public void DrawTexture(Texture2D texture)
        {
            terrainDisplay.spriteRender.enabled = true;
            terrainDisplay.DrawTextureRuntime(texture);
        }

        public sbyte GetCellLayer(int x, int y)
        {
            int adjustedX = x < 0 ? cellsInLine + x : x >= cellsInLine ? x - cellsInLine : x;
            int adjustedY = y < 0 ? cellsInLine + y : y >= cellsInLine ? y - cellsInLine : y;

            ActiveChunk chunk = neighboursMatrix[matrixCenter.x + (x < 0 ? -1 : x >= cellsInLine ? 1 : 0), matrixCenter.y + (y < 0 ? -1 : y >= cellsInLine ? 1 : 0)];      
            sbyte layer = chunk.RawLayersMatrix.matrix[adjustedX, adjustedY];
                
            return layer;
        }
    }
}
