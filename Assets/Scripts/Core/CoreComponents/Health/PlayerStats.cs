using Lean.Pool;
using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public class PlayerStats : Stats
    {
        [SerializeField] private float _lowHealthPercent;

        public static event Action<int> OnIncreaseHealth, OnDecreaseHealth;
        public static event Action<int, int> OnHealthChanged;

        protected override bool ExposeProperties => true;

        #region Health Funcs
        public override void DecreaseHealth(int amount)
        {
            if (CurrentHealth == 0) 
            {
                return;
            }
            base.DecreaseHealth(amount);

            OnDecreaseHealth?.Invoke(CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public override void IncreaseHealth(int amount)
        {
            base.IncreaseHealth(amount);

            OnIncreaseHealth?.Invoke(CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }


        #endregion
    }
}