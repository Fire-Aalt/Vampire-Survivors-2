using Game.CoreSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game
{
    public class Hitbox : MonoBehaviour
    {
        [Title("Behavior")]
        [SerializeField] private bool _deactivateOnZeroHealth;
        [SerializeField, ShowIf("_deactivateOnZeroHealth")] private Stats _stats;
        [Header("Callbacks")]
        [SerializeField] private bool _onEnter;
        [SerializeField] private bool _onStay;
        [SerializeField] private bool _onExit;
        [Title("Damage")]
        [SerializeField] private bool _damagePlayer;
        [ShowIf("_damagePlayer")] public int damageToPlayer;

        [SerializeField] private bool _damageEntities;
        [ShowIf("_damageEntities")] public int damageToEntities;

        [SerializeField, ShowIf("_damageEntities")] private bool _applyKnockback;
        [SerializeField, ShowIf("@_damageEntities && _applyKnockback")] private float knockbackStrength;

        public event Action<Collider2D> OnEnter, OnStay, OnExit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_onEnter)
                return;

            OnEnter?.Invoke(collision);
            PerformHit(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_onStay)
                return;

            OnStay?.Invoke(collision);
            PerformHit(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!_onExit)
                return;

            OnExit?.Invoke(collision);
            PerformHit(collision);
        }

        private void PerformHit(Collider2D collider)
        {
            if (_deactivateOnZeroHealth && !_stats.IsAlive)
                return;

            if (_damagePlayer || _damageEntities)
            {
                if (collider.TryGetComponent(out IDamageable damageable))
                {
                    if (_damagePlayer && collider.CompareTag("Player"))
                        damageable.Damage(this, damageToPlayer, out int dealtDamage);
                    if (_damageEntities && !collider.CompareTag("Player"))
                        damageable.Damage(this, damageToEntities, out int dealtDamage);
                }
            }

            if (_applyKnockback && collider.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector2 direction = (collider.transform.position - transform.position).normalized;

                knockbackable.Knockback(direction, knockbackStrength);
            }
        }
    }
}