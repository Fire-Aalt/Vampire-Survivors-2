using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game
{
    public class OutOfBoundsDespawner : MonoBehaviour, IPoolable
    {
        [Title("Despawn Config")]
        [SerializeField] private float _despawnTime;

        [Title("Visible Circle")]
        public float radius;
        public Vector2 offset;

        public event Action OnVisible, OnHidden;

        public bool IsVisible { get; private set; }

        private Area _visibleArea;
        private Timer _despawnTimer;

        private void Awake()
        {
            _despawnTimer = new Timer();
            _despawnTimer.OnTimerDone += Despawn;            
        }

        void Start()
        {
            _visibleArea = EnemyManager.WorldVisibleArea;
        }

        void Update()
        {
            _despawnTimer.Tick();

            if (!IsVisible && !_despawnTimer.IsActive)
            {
                _despawnTimer.StartTimer();
                OnHidden?.Invoke();
            }
            else if (IsVisible && _despawnTimer.IsActive)
            {
                _despawnTimer.StopTimer();
                OnVisible?.Invoke();
            }

            IsVisible = CheckIfVisible();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3)offset, radius);
        }

        private bool CheckIfVisible()
        {
            Vector2 point = (Vector2)transform.position + offset;
            float outerDistance = _visibleArea.OuterDistance(point);

            if (outerDistance <= radius)
            {
                return true;
            }
            return false;
        }

        private void Despawn() => LeanPool.Despawn(gameObject);

        public void OnSpawn()
        {
            _despawnTimer.StartTimer(_despawnTime);
        }

        public void OnDespawn()
        {
            _despawnTimer.StopTimer();
        }
    }
}
