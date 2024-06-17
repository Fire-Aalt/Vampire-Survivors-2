using Lean.Pool;
using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public class EnemyStats : Stats
    {
        public event Action<Enemy> OnEnemyDeath;

        protected override bool ExposeProperties => true;

        protected Enemy Enemy { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Enemy = GetComponentInParent<Enemy>(true);
        }

        private void HandleHit()
        {
            // Not Implemented
        }

        private void HandleDeath()
        {
            OnEnemyDeath?.Invoke(Enemy);
        }

        private void OnEnable()
        {
            OnHit += HandleHit;
            OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            OnHit -= HandleHit;
            OnDeath -= HandleDeath;
        }
    }
}
