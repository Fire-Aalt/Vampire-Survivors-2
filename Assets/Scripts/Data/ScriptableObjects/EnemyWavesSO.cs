using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Enemy Waves Data")]
    public class EnemyWavesSO : ScriptableObject
    {
        public List<Wave> waves = new();

        private List<float[]> _cachedChances;

        public void Initialize()
        {
            _cachedChances = new List<float[]>();
            for (int i = 0; i < waves.Count; i++)
            {
                _cachedChances.Add(new float[waves[i].enemiesInWaves.Count]);
            }
        }

        public Enemy GetNextEnemy(int wave, float t)
        {
            float totalChance = 0f;
            var enemies = waves[wave].enemiesInWaves;
            for (int enemyId = 0; enemyId < enemies.Count; enemyId++)
            {
                float chance = enemies[enemyId].appearenceCurve.Evaluate(t);
                _cachedChances[wave][enemyId] = chance;
                totalChance += chance;
            }

            float randomValue = Random.value;
            float sum = 0f;

            var chances = _cachedChances[wave];
            for (int i = 0; i < chances.Length; i++)
            {
                sum += chances[i] / (float)totalChance;
                if (sum >= randomValue)
                {
                    return waves[wave].enemiesInWaves[i].enemyPrefab;
                }
            }
            return null;
        }

        public float GetNextEnemiesInWaveDuration(int waveId, float t)
        {
            Wave wave = waves[waveId];
            float normDuration = wave.enemiesInWaveIncreaseDurationRange.y - wave.enemiesInWaveIncreaseDurationRange.x;
            return (1 - wave.enemiesInWaveCurve.Evaluate(t)) * normDuration + wave.enemiesInWaveIncreaseDurationRange.x;
        }
    }

    [System.Serializable]
    public class Wave
    {
        [MinValue(1f)] public float duration = 60;
        [MinValue(0)] public int enemiesInWaveAtStart = 3;
        [MinValue(0)] public int enemiesInWaveIncreaseAmount = 1;
        [MinValue(0)] public Vector2 enemiesInWaveIncreaseDurationRange = new(1f, 3f);
        public AnimationCurve enemiesInWaveCurve;
        public List<EnemyInWave> enemiesInWaves = new();
    }

    [System.Serializable]
    public class EnemyInWave
    {
        [AssetsOnly, AssetSelector(Paths = "Assets/Prefabs/Enemies")]
        public Enemy enemyPrefab;
        public AnimationCurve appearenceCurve;
    }
}
