using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Ability : MonoBehaviour
    {
        [field: SerializeField] public AbilitySO Data { get; private set; }
        [SerializeField] protected SoundEffectSO triggerSound;

        protected Timer durationTimer;
        protected Timer cooldownTimer;
        protected AbilityController controller;

        protected List<Projectile> activeProjectiles = new();

        public void SetController(AbilityController controller) => this.controller = controller;

        public virtual void Initialize()
        {
            durationTimer = new Timer();
            cooldownTimer = new Timer();
            durationTimer.OnTimerDone += StopAbility;
            cooldownTimer.OnTimerDone += TriggerAbility;

            cooldownTimer.StartTimer(Data.Cooldown.Value);
        }

        public virtual void LogicUpdate()
        {
            durationTimer.Tick();
            cooldownTimer.Tick();
        }

        public virtual UpgradeInfo GetUpgradeInfo(int level)
        {
            return Data.upgradeInfos[level];
        }

        protected virtual void TriggerAbility()
        {
            durationTimer.StartTimer(Data.Duration.Value);
        }

        protected virtual void StopAbility()
        {
            cooldownTimer.StartTimer(Data.Cooldown.Value);
        }

        protected virtual T SpawnProjectile<T>(T prefab, Vector2 position, Quaternion rotation) where T : Projectile
        {
            var projectile = LeanPool.Spawn(prefab, position, rotation);

            activeProjectiles.Add(projectile);
            projectile.OnDespawnEvent += DisposeProjectile;

            return projectile;
        }

        protected virtual void DisposeProjectile(Projectile projectile)
        {
            activeProjectiles.Remove(projectile);
        }

        protected bool DamageCollider(Collider2D collider, int damage, out int dealtDamage)
        {
            if (collider.TryGetComponent(out IDamageable damageable))
            {
                if (damageable.Damage(this, damage, out dealtDamage))
                {
                    return true;
                }
            }
            dealtDamage = 0;
            return false;
        }

        protected void KnockbackCollider(Collider2D collider, float knockback)
        {
            if (collider.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector2 direction = (collider.transform.position - transform.position).normalized;
                knockbackable.Knockback(direction, knockback);
            }
        }
    }
}
