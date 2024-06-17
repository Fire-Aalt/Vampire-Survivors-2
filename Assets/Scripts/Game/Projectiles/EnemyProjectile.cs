using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyProjectile : Projectile
    {
        [SerializeField] protected ParticleSystem hitParticle;

        protected float speed;
        protected int damage;
        protected float knockback;

        public void SetParams(float speed, float damage, float knockback)
        {
            this.speed = speed;
            this.damage = Mathf.RoundToInt(damage);
            this.knockback = knockback;
        }

        protected void Update()
        {
            Vector2 direction = transform.right;
            rb.velocity = direction * speed;
        }

        protected void HandleColliderEnter(Collider2D collider)
        {
            if (DamageCollider(collider, damage))
            {
                KnockbackCollider(collider, knockback);
            }
            LeanPool.Spawn(hitParticle, transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 270f));
            Despawn();
        }

        private void OnEnable()
        {
            hitbox.OnEnter += HandleColliderEnter;
        }

        private void OnDisable()
        {
            hitbox.OnEnter -= HandleColliderEnter;
        }
    }
}
