using Game.CoreSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class PistolAbility : Ability
    {
        [SerializeField, AssetsOnly]
        protected Bullet _bulletPrefab;

        [SerializeField] private Transform _pistolHandle;
        [SerializeField] private SpriteRenderer _pistolSprite;
        [SerializeField] private Transform _bulletSpawnPoint;

        protected Stat BulletSpeed => Data.GetStat("BulletSpeed");
        protected Stat BulletDamage => Data.GetStat("BulletDamage");
        protected Stat BulletKnockback => Data.GetStat("BulletKnockback");
        protected Stat Piercing => Data.GetStat("Piercing");
        protected Stat Spread => Data.GetStat("Spread");

        protected Movement _playerMovement;
        protected Stats _playerStats;

        public override void Initialize()
        {
            base.Initialize();

            _playerMovement = Player.Active.Core.GetCoreComponent<Movement>();
            _playerStats = Player.Active.Core.GetCoreComponent<Stats>();
            _playerMovement.OnFlipped += RotatePistol;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_playerStats.IsAlive)
            {
                return;
            }

            RotatePistol();
        }

        protected override void TriggerAbility()
        {
            base.TriggerAbility();

            if (!Player.IsAlive) return;

            triggerSound.Play(transform.position);
            Vector3 dir = Player.AimDirection;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle += Random.Range(-Spread.Value, Spread.Value);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            var bullet = SpawnProjectile(_bulletPrefab, _bulletSpawnPoint.position, rotation);
            bullet.SetParams(BulletSpeed.Value, BulletDamage.Value * Data.Power.Value, BulletKnockback.Value, (int)Piercing.Value);
            bullet.Initialize(transform.position);
        }

        private void RotatePistol()
        {
            Vector2 dir = Player.AimDirection;
            Vector3 euler = _pistolHandle.rotation.eulerAngles;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if ((angle > 90f && angle < 270f) || (angle < -90 && angle > -270))
            {
                angle = 180 - angle;
            }

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _pistolHandle.rotation = Quaternion.Euler(euler.x, euler.y, rotation.eulerAngles.z);
        }
    }
}
