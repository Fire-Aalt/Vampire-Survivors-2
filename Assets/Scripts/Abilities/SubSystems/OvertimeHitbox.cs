using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class OvertimeHitbox : MonoBehaviour
    {
        [SerializeField] private List<Collider2D> _colliders = new();
        [SerializeField] private int _bufferSize = 25;
        [SerializeField] private LayerMask _hitLayerMask;

        public Timer DurationTimer { get; private set; }
        public Timer AttackCooldownTimer { get; private set; }
        public event Action<Collider2D, int> OnColliderDamaged;

        private ContactFilter2D _contactFilter;
        private Collider2D[] _buffer;

        private int _damage;
        private float _knockback;

        private void Start()
        {
            _contactFilter = new();
            _contactFilter.NoFilter();
            _contactFilter.SetLayerMask(_hitLayerMask);
            _buffer = new Collider2D[_bufferSize];

            DurationTimer = new Timer();
            AttackCooldownTimer = new Timer(repeat: true);
            DurationTimer.OnTimerDone += DeactivateHitBox;
            AttackCooldownTimer.OnTimerDone += Damage;
        }

        public void Initialize(float damage, float knockback)
        {
            this._damage = Mathf.RoundToInt(damage);
            this._knockback = knockback;
        }

        private void Update()
        {
            DurationTimer.Tick();
            AttackCooldownTimer.Tick();
        }

        public void ActivateHitbox(float duration, float cooldown)
        {
            Damage();
            DurationTimer.StartTimer(duration);
            AttackCooldownTimer.StartTimer(cooldown);
        }

        public void DeactivateHitBox()
        {
            AttackCooldownTimer.StopTimer();
        }

        public void Damage()
        {
            foreach (var collider in _colliders)
            {
                int hitAmount = collider.OverlapCollider(_contactFilter, _buffer);
                for (int i = 0; i < hitAmount; i++)
                {
                    var target = _buffer[i];
                    if (DamageCollider(target, _damage, out int dealtDamage))
                    {
                        KnockbackCollider(target, _knockback);
                        OnColliderDamaged?.Invoke(target, dealtDamage);
                    }
                }
            }
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
