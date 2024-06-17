using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LaserBeamAbility : Ability
    {
        [SerializeField, AssetsOnly]
        private LaserBeamProjectile _projectile;

        private List<Enemy> _enemies;

        protected Stat BeamDamage => Data.GetStat("BeamDamage");
        protected Stat BeamKnockback => Data.GetStat("BeamKnockback");
        protected Stat BeamDamageCooldown => Data.GetStat("BeamDamageCooldown");
        protected Stat NextBeamTriggerCooldown => Data.GetStat("NextBeamTriggerCooldown");

        public override void Initialize()
        {
            base.Initialize();

            _enemies = EnemyManager.VisibleEnemies;
        }

        protected override void TriggerAbility()
        {
            base.TriggerAbility();

            if (!Player.IsAlive) return;
            if (_enemies.Count < 1) return;

            SummonBeamsTask().Forget();
        }

        private async UniTaskVoid SummonBeamsTask()
        {
            int quantity = Mathf.FloorToInt(Data.Quantity.Value);
            for (int i = 0; i < quantity; i++)
            {
                Enemy enemy = _enemies[Random.Range(0, _enemies.Count)];

                var laserBeam = SpawnProjectile(_projectile, enemy.transform.position, Quaternion.identity);
                laserBeam.SetParams(Data.Duration.Value, BeamDamage.Value * Data.Power.Value, BeamKnockback.Value, BeamDamageCooldown.Value);
                laserBeam.Initialize(enemy.transform.position);
                triggerSound.Play(laserBeam.transform.position);

                await UniTask.WaitForSeconds(NextBeamTriggerCooldown.Value, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            }
        }
    }
}
