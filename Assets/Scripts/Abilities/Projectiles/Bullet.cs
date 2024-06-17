using Lean.Pool;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Game
{
    public class Bullet : Projectile
    {
        [SerializeField] protected ParticleSystem hitParticle;

        protected float speed;
        protected int damage;
        protected float knockback;
        protected int piercing;

        protected int piercedEnemies;

        public void SetParams(float speed, float damage, float knockback, int piercing)
        {
            this.speed = speed;
            this.damage = Mathf.RoundToInt(damage);
            this.knockback = knockback;
            this.piercing = piercing;
        }

        public override void Initialize(Vector3 knockbackOrigin)
        {
            base.Initialize(knockbackOrigin);

            piercedEnemies = 0;
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
                hitSound.Play(transform.position);
                KnockbackCollider(collider, knockback);
                piercedEnemies++;
                LeanPool.Spawn(hitParticle, transform.position, Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 270f));
                if (piercing < piercedEnemies)
                {
                    Despawn();
                }
            }
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
