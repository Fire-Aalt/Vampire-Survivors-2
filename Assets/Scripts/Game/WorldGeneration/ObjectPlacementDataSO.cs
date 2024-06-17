using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Object Placement Data")]
    public class ObjectPlacementDataSO : ScriptableObject
    {
        public List<ObjectPlacementLayer> layers = new();
        private Dictionary<int, int> _weightsSums;

        public void PrecalculateWeightSum()
        {
            _weightsSums = new Dictionary<int, int>();
            for (int i = 0; i < layers.Count; i++)
            {
                _weightsSums.Add(i, layers[i].objects.Sum(t => t.weight));
            }
        }

        public GameObject GetWeightedObject(int layer)
        {
            float randomValue = Random.value;
            float sum = 0f;

            int total = _weightsSums[layer];
            if (total == 0)
            {
                Debug.LogError("No precalculation was ran! Source: " + this);
                return null;
            }

            ObjectPlacementLayer objectLayer = layers[layer];
            for (int i = 0; i < objectLayer.objects.Count; i++)
            {
                sum += objectLayer.objects[i].weight / (float)total;
                if (sum >= randomValue)
                {
                    return objectLayer.objects[i].prefab;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class ObjectPlacementLayer
    {
        [Title("Noise")]
        public NoiseType noiseType = NoiseType.Single;
        [ShowIf("noiseType", NoiseType.Layered)] public NoiseDataSO layeredNoise;
        [ShowIf("noiseType", NoiseType.Single)] public NoiseSettings singleNoise;

        [Title("Objects To Place")]
        public List<ObjectToPlace> objects = new();

        [Title("Per Chunk Settings")]
        public int maxObjects = 10;
        public int placementTries = 100;
    }

    [System.Serializable]
    public class ObjectToPlace
    {
        public GameObject prefab;
        [MinValue(1)] public int weight = 1;
        public AnimationCurve heightProbabilityCurve;
    }

    public enum NoiseType 
    {
        Single,
        Layered
    }
}
