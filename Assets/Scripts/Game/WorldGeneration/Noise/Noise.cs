using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Noise
    {
        public enum NormalizeMode { Local, Global };

        public static float[,] GenerateLayeredNoiseMap(int mapSize, List<NoiseLayer> noiseLayers, int seed, Vector2 offset, NormalizeMode normalizeMode)
        {
            float[,] noiseMap = new float[mapSize, mapSize];

            float minLocalNoiseHeight = float.MaxValue;
            float maxLocalNoiseHeight = float.MinValue;

            for (int i = 0; i < noiseLayers.Count; i++)
            {
                NoiseLayer noiseLayer = noiseLayers[i];
                float[,] layer = GenerateNoiseMap(mapSize, noiseLayer.settings, seed, offset, normalizeMode);

                for (int y = 0; y < mapSize; y++)
                {
                    for (int x = 0; x < mapSize; x++)
                    {
                        noiseMap[x, y] += layer[x, y] * noiseLayer.factor;
                        float noiseHeight = noiseMap[x, y];

                        if (noiseHeight > maxLocalNoiseHeight)
                        {
                            maxLocalNoiseHeight = noiseHeight;
                        }
                        else if (noiseHeight < minLocalNoiseHeight)
                        {
                            minLocalNoiseHeight = noiseHeight;
                        }
                    }
                }
            }

            return noiseMap;
        }

        public static float[,] GenerateNoiseMap(int mapSize, NoiseSettings settings, int seed, Vector2 offset, NormalizeMode normalizeMode)
        {
            float[,] noiseMap = new float[mapSize, mapSize];

            System.Random prng = new(seed);
            Vector2[] octaveOffsets = new Vector2[settings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = settings.amplitude;
            for (int i = 0; i < settings.octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + settings.offset.x + offset.x;
                float offsetY = prng.Next(-100000, 100000) + settings.offset.y + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapSize / 2f;
            float halfHeight = mapSize / 2f;

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    amplitude = settings.amplitude;
                    float frequency = settings.frequency;
                    float noiseHeight = 0;

                    for (int i = 0; i < settings.octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                        //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            NormalizeNoiseMap(noiseMap, minLocalNoiseHeight, maxLocalNoiseHeight, settings, normalizeMode);

            return noiseMap;
        }

        private static void NormalizeNoiseMap(float[,] noiseMap, float minLocalNoiseHeight, float maxLocalNoiseHeight, NoiseSettings settings, NormalizeMode normalizeMode)
        {
            int mapSize = noiseMap.GetLength(0);

            float maxPossibleHeight = 0;
            float amplitude = settings.amplitude;
            for (int i = 0; i < settings.octaves; i++)
            {
                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                    else
                    {
                        //float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                        //noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);

                        noiseMap[x, y] = Mathf.InverseLerp(0, maxPossibleHeight, noiseMap[x, y]);

                        //noiseMap[x, y] = Mathf.InverseLerp(-maxPossibleHeight, maxPossibleHeight, noiseMap[x, y]);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class NoiseSettings
    {
        public int mapSize = 16;
        [MinValue(0.001f)] public float scale = 10;
        public Vector2Int offset;

        [Range(1, 20)] public int octaves = 3;

        [MinValue(0f)] public float amplitude = 1f;
        [Tooltip("Persistance determines how amplitude is changed")]
        [Range(0f, 1f)] public float persistance = 0.5f;

        [MinValue(0f)] public float frequency = 1f;
        [Tooltip("Lacunarity determines how frequency is changed")]
        [Range(1f, 10f)] public float lacunarity = 2f;
    }
}