using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public class Stats : CoreComponent
    {
        public event Action OnHit, OnDeath;

        [Title("Exposed Properties")]
        protected virtual bool ExposeMaxHealth => true;
        [SerializeField, Min(1), ShowIf("@ExposeMaxHealth && ExposeProperties")] private int _maxHealth;

        protected virtual bool ExposeDefence => true;
        [SerializeField, Min(0), ShowIf("@ExposeDefence && ExposeProperties")] private int _defence;

        protected virtual bool ExposeProperties => true;

        public bool IsAlive { get => CurrentHealth > 0; }

        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int Defense { get => _defence; set => _defence = value; }

        public int CurrentHealth { get; protected set; }

        protected override void Start()
        {
            base.Start();

            Revive(1f);
        }

        public virtual void Revive(float healthPercent)
        {
            CurrentHealth = Mathf.FloorToInt(MaxHealth * healthPercent);
        }

        public virtual void IncreaseHealth(int amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        }

        public virtual void DecreaseHealth(int amount)
        {
            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;

                OnDeath?.Invoke();
            }
            else
            {
                OnHit?.Invoke();
            }
        }
    }
}
