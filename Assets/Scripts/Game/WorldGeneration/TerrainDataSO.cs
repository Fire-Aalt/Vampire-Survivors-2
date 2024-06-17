using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/TerrainData")]
    public class TerrainDataSO : ScriptableObject
    {
        public List<TerrainType> terrainTypes = new();
        public NoiseToTilesSettings noiseToTilesSettings;
    }

    [System.Serializable]
    public struct TerrainType
    {
        public float startHeight;
        public Color color;
    }
}
