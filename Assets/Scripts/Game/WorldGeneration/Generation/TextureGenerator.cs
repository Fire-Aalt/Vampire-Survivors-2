using UnityEngine;

namespace Game
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color32[,] colorMap, int scale)
        {
            Color32[] convertedColorMap = ScaleAndConvert(colorMap, scale, out int scaledWidth, out int scaledHeight);

            Texture2D texture = new(scaledWidth, scaledHeight)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            texture.SetPixels32(convertedColorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap, int scale)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color32[,] colorMap = new Color32[width, height];
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    colorMap[x, y] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }

            return TextureFromColorMap(colorMap, scale);
        }

        private static Color32[] ScaleAndConvert(Color32[,] colorMap, int scale, out int scaledWidth, out int scaledHeight)
        {
            int unscaledWidth = colorMap.GetLength(0);
            int unscaledHeight = colorMap.GetLength(1);

            scaledWidth = unscaledWidth * scale;
            scaledHeight = unscaledHeight * scale;
            Color32[] scaledColorMap = new Color32[scaledWidth * scaledHeight];
            for (int y = 0; y < scaledWidth; y++)
            {
                for (int x = 0; x < scaledHeight; x++)
                {
                    int unscaledX = x / scale;
                    int unscaledY = y / scale;

                    scaledColorMap[y * scaledWidth + x] = colorMap[unscaledX, unscaledY];
                }
            }
            return scaledColorMap;
        }
    }
}
