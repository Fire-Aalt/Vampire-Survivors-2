using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public abstract class Hurtbox : CoreComponent, IDamageable
    {
        public const float DEFENCE_FACTOR = 50 / 3;

        [Title("Default Properties")]
        [SerializeField] protected bool isObject;

        [Title("Exposed Properties")]
        protected virtual bool ExposeIFramesDuration => true;
        [SerializeField, Min(0.2f), ShowIf("ExposeIFramesDuration")] private float _iFramesDuration;

        public float IFramesDuration { get => _iFramesDuration; set => _iFramesDuration = value; }

        public bool IsDamageable { get => Stats?.IsAlive ?? true; }
        public bool IsObject { get => isObject; }

        public Stats Stats { get; private set; }
        public Invincibility Invincibility { get; private set; }

        public event Action OnHit;
        protected abstract bool UseInvincibility { get; }

        protected override void Awake()
        {
            base.Awake();

            if (core.HasCoreComponent<Stats>())
            {
                Stats = core.GetCoreComponent<Stats>();
            }

            Invincibility = new Invincibility();
        }

        public virtual bool Damage(object source, int amount, out int dealtDamage, int piercing = 0)
        {
            if ((Stats == null || !Stats.IsAlive) || (UseInvincibility && _iFramesDuration > 0f && Invincibility.IsInvincible))
            {
                dealtDamage = 0;
                return false;
            }

            OnHit?.Invoke();

            dealtDamage = CalculateDamage(amount, Stats.Defense, piercing);
            Stats.DecreaseHealth(dealtDamage);
            if (UseInvincibility)
            {
                HitInvincibility();
            }
            return true;
        }

        public static int CalculateDamage(int damage, int defence, int piercing = 0)
        {
            defence = Mathf.Clamp(defence - piercing, 0, int.MaxValue);
            return Mathf.Clamp(Mathf.FloorToInt(damage - damage * CalculateDamageReduction(defence)), 1, int.MaxValue);
        }

        public static float CalculateDamageReduction(int defence)
        {
            return defence / (defence + DEFENCE_FACTOR);
        }

        protected virtual void HitInvincibility()
        {
            Invincibility.StartHitInvincibility(_iFramesDuration);
        }
    }
}
