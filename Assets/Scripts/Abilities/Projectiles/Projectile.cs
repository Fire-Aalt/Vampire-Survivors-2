using Lean.Pool;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        [Title("Hitbox")]
        [SerializeField] protected Hitbox hitbox;
        [SerializeField] protected MMF_Player hitFeedback;
        [SerializeField] protected SoundEffectSO hitSound;

        [Title("Movable Projectile")]
        [SerializeField] protected bool isMovable;
        [SerializeField, ShowIf("isMovable")] protected Rigidbody2D rb;
        [SerializeField, ShowIf("isMovable")] protected TrailRenderer trailRenderer;

        public event Action<Projectile> OnDespawnEvent;

        protected Vector3 knockbackOrigin;
        protected MMF_FloatingText floatingText;
        protected bool hasFloatingText;
        protected bool isAlive;

        protected Dictionary<IDamageable, bool> _damagedEntities;

        protected virtual void Awake()
        {
            floatingText = hitFeedback.GetFeedbackOfType<MMF_FloatingText>();
            hasFloatingText = floatingText != null;
            _damagedEntities = new Dictionary<IDamageable, bool>();
        }

        public virtual void Initialize(Vector3 knockbackOrigin)
        {
            this.knockbackOrigin = knockbackOrigin;
        }

        public virtual void OnSpawn()
        {
            if (isMovable)
            {
                trailRenderer.Clear();
                trailRenderer.emitting = true;
            }
            _damagedEntities.Clear();
            isAlive = true;
        }

        public virtual void OnDespawn()
        {
            if (isMovable)
            {
                trailRenderer.emitting = false;
            }
            isAlive = false;
            OnDespawnEvent?.Invoke(this);
        }

        protected void Despawn() => LeanPool.Despawn(gameObject);

        protected bool DamageCollider(Collider2D collider, int damage)
        {
            if (collider.TryGetComponent(out IDamageable damageable) && !_damagedEntities.ContainsKey(damageable))
            {
                if (damageable.Damage(this, damage, out int dealtDamage))
                {
                    _damagedEntities.Add(damageable, true);
                    if (hasFloatingText)
                    {
                        floatingText.Value = dealtDamage.ToString();
                    }
                    hitFeedback.PlayFeedbacks();
                    return true;
                }
            }
            return false;
        }

        protected void KnockbackCollider(Collider2D collider, float knockback)
        {
            if (collider.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector2 direction = (collider.transform.position - knockbackOrigin).normalized;
                knockbackable.Knockback(direction, knockback);
            }
        }
    }
}