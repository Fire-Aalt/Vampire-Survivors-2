using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyManager : MonoBehaviour
    {
        public const float TRACK_COOLDOWN = 0.1f; 

        public static List<Enemy> VisibleEnemies { get; private set; }

        public static Bounds VisibleAreaBounds { get; private set; }
        public static Area WorldVisibleArea { get; private set; }
        public static Enemy ClosestEnemyToPlayer { get; private set; }

        private Camera _camera;

        private void Awake()
        {
            VisibleEnemies = new List<Enemy>();

            _camera = CameraManager.Active.MainCamera;
            VisibleAreaBounds = new Bounds()
            {
                size = CameraManager.Active.Bounds.size,
            };
            
            WorldVisibleArea = new(VisibleAreaBounds);
            TrackClosestEnemy().Forget();
        }

        private void Update()
        {
            WorldVisibleArea.position = (Vector2)_camera.transform.position;
        }

        public void HandleEnemyVisible(Enemy enemy)
        {
            VisibleEnemies.Add(enemy);
        }

        public void HandleEnemyHidden(Enemy enemy)
        {
            VisibleEnemies.Remove(enemy);
        }

        public void HandleEnemyDeath(Enemy enemy)
        {
            if (VisibleEnemies.Contains(enemy))
            {
                VisibleEnemies.Remove(enemy);
            }
        }

        public static int GetEnemyCount() => VisibleEnemies.Count;

        public static Enemy GetClosestEnemy()
        {
            if (Player.Active == null) return null;

            var playerPos = Player.Active.transform.position;
            Enemy closestEnemy = null;
            float closestDistance = float.PositiveInfinity;

            foreach (var enemy in VisibleEnemies)
            {
                var distance = Vector2.Distance(enemy.transform.position, playerPos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            return closestEnemy;
        }

        private async UniTaskVoid TrackClosestEnemy()
        {
            while (true)
            {
                ClosestEnemyToPlayer = GetClosestEnemy();
                await UniTask.WaitForSeconds(TRACK_COOLDOWN);
            }
        }

        private void OnEnable()
        {
            Enemy.OnVisible += HandleEnemyVisible;
            Enemy.OnHidden += HandleEnemyHidden;
            Enemy.OnDeath += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            Enemy.OnVisible -= HandleEnemyVisible;
            Enemy.OnHidden -= HandleEnemyHidden;
            Enemy.OnDeath -= HandleEnemyDeath;
        }
    }
}
