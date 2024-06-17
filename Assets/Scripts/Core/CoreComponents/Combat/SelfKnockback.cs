using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.CoreSystem
{
    public class SelfKnockback : CoreComponent, IKnockbackable
    {
        [Title("Dependencies")]
        [SerializeField] protected bool _dependOnHealth;
        [SerializeField] protected bool _dependOnHurtbox;

        [ShowIf("_dependOnHurtbox")]
        [SerializeField, Range(0f, 1f)] protected float _durationMultiplier = 1f;
        [HideIf("_dependOnHurtbox")]
        [SerializeField] protected float _duration = 0.2f;

        [Title("Overrides")]
        [InfoBox("Script no longer responds to IKnockbackable callback", InfoMessageType = InfoMessageType.Warning, VisibleIf = "_overrideBehavior")]
        [SerializeField] private bool _overrideBehavior;

        [HorizontalGroup("XAngle")]
        [SerializeField] private bool _overrideXAngle;
        [HorizontalGroup("XAngle")]
        [SerializeField, ShowIf("_overrideXAngle")] private float _xAngle;

        [HorizontalGroup("YAngle")]
        [SerializeField] private bool _overrideYAngle;
        [HorizontalGroup("YAngle")]
        [SerializeField, ShowIf("_overrideYAngle")] private float _yAngle;

        [HorizontalGroup("Strength")]
        [SerializeField] private bool _overrideStrength;
        [HorizontalGroup("Strength")]
        [SerializeField, ShowIf("_overrideStrength")] private float _strength;

        [HorizontalGroup("TerminateVelocity")]
        [SerializeField] private bool _overrideTerminateVelocity;
        [HorizontalGroup("TerminateVelocity")]
        [SerializeField, ShowIf("_overrideTerminateVelocity")] private float _terminateVelocity;

        protected Hurtbox _hurtbox;
        protected Stats _health;

        protected Knockback _knockback;

        protected override void Awake()
        {
            base.Awake();

            if (_dependOnHurtbox)
            {
                if (!TryGetComponent(out _hurtbox)) 
                    Debug.LogError("Hurtbox CoreComponent not found on " + gameObject);
            }
            if (_dependOnHealth)
            {
                _health = core.GetCoreComponent<Stats>();
                if (_health == null)
                    Debug.LogError("Health CoreComponent not found on " + core);
            }

            _knockback = new Knockback(core.GetCoreComponent<Movement>());
        }

        public virtual void Knockback(Vector2 angle, float strength, bool ignoreConditions)
        {
            if (!_overrideBehavior)
                PlayKnockback(angle, strength, 0f, true, ignoreConditions);
        }

        /// <summary>
        /// Parameters can be overriden
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="strength"></param>
        /// <param name="ignoreConditions"></param>
        public virtual void PlayKnockback(Vector2 angle, float strength, float terminateVelocity = 0f, bool ignoreGravity = false, bool ignoreConditions = false)
        {
            if (ignoreConditions || CanKnockback())
            {
                float duration;
                if (_dependOnHurtbox)
                    duration = _hurtbox.IFramesDuration * _durationMultiplier;
                else
                    duration = _duration;

                if (_overrideXAngle)
                    angle = new Vector2(_xAngle, angle.y);
                if (_overrideYAngle)
                    angle = new Vector2(angle.x, _yAngle);
                if (_overrideStrength)
                    strength = _strength;
                if (_overrideTerminateVelocity)
                    terminateVelocity = _terminateVelocity;

                _knockback.ApplyKnockback(angle, strength, duration, terminateVelocity, ignoreGravity);
            }
        }

        protected virtual bool CanKnockback()
        {
            if (_dependOnHealth)
            {
                if (!_health.IsAlive)
                    return false;
            }

            if (_dependOnHurtbox)
            {
                if (_hurtbox.Invincibility.IsActionInvincible || _knockback.IsKnockback)
                    return false;
            }

            return true;
        }
    }
}
