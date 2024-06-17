using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/NoiseData")]
    public class NoiseDataSO : ScriptableObject
    {
        public List<NoiseLayer> noiseLayers = new();
    }

    [System.Serializable]
    public struct NoiseLayer
    {
        public NoiseSettings settings;
        public float factor;
    }
}
