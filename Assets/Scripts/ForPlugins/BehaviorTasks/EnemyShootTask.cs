using BehaviorDesigner.Runtime.Tasks;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyShootTask : EnemyAction
    {
        public EnemyProjectile projectilePrefab;
        public Transform shootOrigin;

        public float projectileSpeed = 2f;
        public float projectileDamage = 2f;
        public float projectileKnockback = 2f;

        public float spread = 15;

        public override TaskStatus OnUpdate()
        {
            Vector3 dir = Enemy.Target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle += Random.Range(-spread, spread);

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            var projectile = LeanPool.Spawn(projectilePrefab, shootOrigin.position, rotation);
            projectile.SetParams(projectileSpeed, projectileDamage, projectileKnockback);
            projectile.Initialize(shootOrigin.position);

            return TaskStatus.Success;
        }
    }
}
