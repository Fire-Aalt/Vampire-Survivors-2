using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LaserBeamProjectile : Projectile
    {
        [Title("Laser Beam")]
        [SerializeField] private ParticlesController _beamController;
        [SerializeField] private ParticlesController _explosionController;
        [SerializeField] private ParticlesController _particlesController;
        [SerializeField] private OvertimeHitbox _overtimeHitbox;
        [SerializeField] private float _delayPerSecond;

        protected float duration;
        protected int damage;
        protected float knockback;
        protected float cooldown;

        private Timer _delayTimer;
        private Timer _durationTimer;

        protected override void Awake()
        {
            base.Awake();

            _delayTimer = new Timer();
            _durationTimer = new Timer();
            _delayTimer.OnTimerDone += ActivateCollider;
            _durationTimer.OnTimerDone += Despawn;           
        }

        public void SetParams(float duration, float damage, float knockback, float cooldown)
        {
            this.duration = duration;
            this.damage = Mathf.RoundToInt(damage);
            this.knockback = knockback;
            this.cooldown = cooldown;

            _overtimeHitbox.Initialize(this.damage, this.knockback);

            float finalDelay = _delayPerSecond * duration;
            _beamController.StopParticles(true);

            _explosionController.ChangeDelay(finalDelay);
            _particlesController.ChangeDelay(finalDelay);

            _beamController.ChangeDuration(duration, true);
            _explosionController.ChangeDuration(duration, true);
            _particlesController.ChangeDuration(duration, false);
        }

        public override void Initialize(Vector3 knockbackOrigin)
        {
            base.Initialize(knockbackOrigin);

            _beamController.PlayParticles(true);
            _delayTimer.StartTimer(_delayPerSecond);
            _durationTimer.StartTimer(duration + 2f);
        }

        private void Update()
        {
            if (isAlive)
            {
                _delayTimer.Tick();
                _durationTimer.Tick();
            }
        }

        private void ActivateCollider()
        {
            _overtimeHitbox.ActivateHitbox(duration - _delayPerSecond, cooldown);
        }

        private void HitFeedback(Collider2D collider, int dealtDamage)
        {
            floatingText.Value = dealtDamage.ToString();
            hitFeedback.PlayFeedbacks(collider.transform.position);
        }

        private void OnEnable()
        {
            _overtimeHitbox.OnColliderDamaged += HitFeedback;
        }

        private void OnDisable()
        {
            _overtimeHitbox.OnColliderDamaged -= HitFeedback;
        }
    }
}
