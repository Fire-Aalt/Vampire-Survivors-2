using Lean.Pool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class EnemyWavesSpawner : MonoBehaviour
    {
        [Title("Data")]
        public EnemyWavesSO EnemyWavesData;
        [SerializeField, ReadOnly] private int _enemiesInWave = 3;

        private Timer _difficultyTimer;
        private Area _spawnArea;

        private int _currentWaveId = -1;
        private float _waveLerpTime;
        private float _waveTime, _waveDuration;

        private bool _isWaveInfinite = false;

        private void Start()
        {
            EnemyWavesData.Initialize();
            _spawnArea = EnemyManager.WorldVisibleArea;

            _difficultyTimer = new Timer();
            _difficultyTimer.OnTimerDone += IncreaseDifficulty;

            StartNextWave();
        }

        private void StartNextWave()
        {
            if (_currentWaveId + 1 < EnemyWavesData.waves.Count)
            {
                _currentWaveId++;
                _enemiesInWave = EnemyWavesData.waves[_currentWaveId].enemiesInWaveAtStart;

                _waveTime = 0f;
                _waveDuration = EnemyWavesData.waves[_currentWaveId].duration;
                _difficultyTimer.StartTimer(EnemyWavesData.GetNextEnemiesInWaveDuration(_currentWaveId, 0f));
            }
            else
            {
                _isWaveInfinite = true;
                Debug.LogWarning("Wave is now Infinite");
            }
        }

        private void Update()
        {
            _difficultyTimer.Tick();

            if (!_isWaveInfinite)
            {
                _waveTime += Time.deltaTime;
                _waveLerpTime = _waveTime / _waveDuration;
                if (_waveTime >= _waveDuration)
                {
                    StartNextWave();
                }
            }

            if (Time.timeScale != 0 && EnemyManager.GetEnemyCount() < _enemiesInWave)
            {
                SpawnEnemy();
            }
        }

        private void IncreaseDifficulty()
        {
            _enemiesInWave += EnemyWavesData.waves[_currentWaveId].enemiesInWaveIncreaseAmount;
            _difficultyTimer.StartTimer(EnemyWavesData.GetNextEnemiesInWaveDuration(_currentWaveId, _waveLerpTime));
        }

        private void SpawnEnemy()
        {
            Enemy enemy = EnemyWavesData.GetNextEnemy(_currentWaveId, _waveLerpTime);

            var enemyDespawner = enemy.Despawner;
            var perimeterPoint = GetRandomPerimeterPos(enemyDespawner.radius, enemyDespawner.offset);
            LeanPool.Spawn(enemy, perimeterPoint, Quaternion.identity);
        }

        private Vector2 GetRandomPerimeterPos(float radius, Vector2 offset)
        {
            var min = new Vector2(_spawnArea.Min.x - radius, _spawnArea.Min.y - radius) + offset;
            var max = new Vector2(_spawnArea.Max.x + radius, _spawnArea.Max.y + radius) - offset;

            int side = Random.Range(0, 4);
            switch (side) 
            {
                case 0:
                    return new Vector2(Random.Range(min.x, max.x), max.y);
                case 1:
                    return new Vector2(Random.Range(min.x, max.x), min.y);
                case 2:
                    return new Vector2(min.x, Random.Range(min.y, max.y));
                case 3:
                    return new Vector2(max.x, Random.Range(min.y, max.y));
                default:
                    goto case 0;
            }
        }
    }
}
