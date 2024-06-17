using Game.CoreSystem;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Game
{
    public class PulseAuraAbility : Ability
    {
        [SerializeField] private ParticlesController _particlesController;
        [SerializeField] private CircleCollider2D _circleCollider;
        [SerializeField] private MMF_Player _hitFeedback;
        [SerializeField] private LayerMask _hitLayerMask;

        protected Stat AuraDamage => Data.GetStat("AuraDamage");
        protected Stat AuraKnockback => Data.GetStat("AuraKnockback");
        protected Stat AuraDamageCooldown => Data.GetStat("AuraDamageCooldown");

        private ContactFilter2D _contactFilter = new();
        private Collider2D[] _buffer;
        private float _startRadius;
        private float _startDamageCooldown;

        private MMF_FloatingText _floatingText;

        private Timer _damageTimer;

        public override void Initialize()
        {
            base.Initialize();

            _contactFilter.NoFilter();
            _contactFilter.SetLayerMask(_hitLayerMask);
            _buffer = new Collider2D[100];
            _startRadius = _circleCollider.radius;
            _startDamageCooldown = AuraDamageCooldown.Value;
            _circleCollider.enabled = false;

            _floatingText = _hitFeedback.GetFeedbackOfType<MMF_FloatingText>();

            _damageTimer = new Timer(repeat: true);
            _damageTimer.OnTimerDone += Damage;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _damageTimer.Tick();
        }

        protected override void TriggerAbility()
        {
            base.TriggerAbility();

            if (!Player.IsAlive) return;

            triggerSound.Play(transform.position, attachToTransform: transform);
            _damageTimer.StartTimer(AuraDamageCooldown.Value);
            ActivateArea();
        }

        private void ActivateArea()
        {
            _particlesController.ChangeDuration(Data.Duration.Value, true);
            _particlesController.ChangeEmmisionScale(Data.Size.Value);
            _particlesController.ChangeSpeedModifier(_startDamageCooldown / AuraDamageCooldown.Value);
            _particlesController.PlayParticles(false);
            _circleCollider.radius = _startRadius * Data.Size.Value;
            _circleCollider.enabled = true;
        }

        private void Damage()
        {
            int hitAmount = _circleCollider.OverlapCollider(_contactFilter, _buffer);
            for (int i = 0; i < hitAmount; i++)
            {
                var collider = _buffer[i];
                int damage = (int)Mathf.Floor(AuraDamage.Value * Data.Power.Value);
                if (DamageCollider(collider, damage, out int dealtDamage))
                {
                    _floatingText.Value = dealtDamage.ToString();
                    _hitFeedback.PlayFeedbacks(collider.transform.position);
                
                    KnockbackCollider(collider, AuraKnockback.Value);
                }
            }
        }
        protected override void StopAbility()
        {
            base.StopAbility();

            triggerSound.StopLoopingSound(true);
            _damageTimer.StopTimer();
            _circleCollider.enabled = false;
        }
    }
}
