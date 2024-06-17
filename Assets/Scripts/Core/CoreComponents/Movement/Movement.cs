using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public class Movement : CoreComponent
    {
        private const float GRAVITY_SCALE = 0;
        public event Action OnFlipped;

        [SerializeField] private int _startingFacingDirection = 1;
        [SerializeField] private Rigidbody2D _rigidbody;
        public Rigidbody2D RB { get; private set; }

        public bool CanSetVelocity { get; set; }

        public int FacingDirection { get; private set; }
        public Vector2 CurrentVelocity { get; private set; }
        public float Magnitude { get; private set; }
        public float VelocityMultiplier { get; set; }

        private Vector2 workspace;

        protected override void Awake()
        {
            base.Awake();

            if (_rigidbody != null)
                RB = _rigidbody;
            else
                RB = GetComponentInParent<Rigidbody2D>();

            RB.gravityScale = GRAVITY_SCALE;
            CanSetVelocity = true;
            FacingDirection = _startingFacingDirection;
            VelocityMultiplier = 1f;
        }

        public override void LogicUpdate()
        {
            UpdateMovementData();
        }

        #region Set Functions

        public void SetVelocityZero(bool ignoreRestriction = false)
        {
            workspace = Vector2.zero;
            SetFinalVelocity(ignoreRestriction);
        }

        public void SetVelocity(float velocity, Vector2 angle, bool ignoreRestriction = false)
        {
            angle.Normalize();
            workspace = new Vector2(angle.x * velocity, angle.y * velocity);
            SetFinalVelocity(ignoreRestriction);
        }

        public void SetVelocity(Vector2 velocity, bool ignoreRestriction = false)
        {
            workspace = new Vector2(velocity.x, velocity.y);
            SetFinalVelocity(ignoreRestriction);
        }

        public void SetVelocityX(float velocity, bool ignoreRestriction = false)
        {
            workspace = new Vector2(velocity, CurrentVelocity.y);
            SetFinalVelocity(ignoreRestriction);
        }

        public void SetVelocityY(float velocity, bool ignoreRestriction = false)
        {
            workspace = new Vector2(CurrentVelocity.x, velocity);
            SetFinalVelocity(ignoreRestriction);
        }

        public void AddVelocity(Vector2 velocity, bool ignoreRestriction = false)
        {
            workspace = new Vector2(CurrentVelocity.x + velocity.x, CurrentVelocity.y + velocity.y);
            SetFinalVelocity(ignoreRestriction);
        }

        public void AddForce(Vector2 velocity, ForceMode2D mode, bool ignoreRestriction)
        {
            if (ignoreRestriction || CanSetVelocity)
            {
                RB.AddForce(velocity * VelocityMultiplier, mode);
                UpdateMovementData();
            }
        }

        private void SetFinalVelocity(bool ignoreRestriction)
        {
            if (CanSetVelocity || ignoreRestriction)
            {
                workspace *= VelocityMultiplier;
                RB.velocity = workspace;
                UpdateMovementData();
            }
        }

        protected void UpdateMovementData()
        {
            CurrentVelocity = RB.velocity;
            Magnitude = CurrentVelocity.magnitude;
        }
        #endregion


        public void Accelerate(float accelRate, Vector2 targetSpeed)
        {
            Vector2 speedDif = targetSpeed - CurrentVelocity;
            Vector2 movement = speedDif * accelRate;

            AddForce(movement, ForceMode2D.Force, false);
        }

        public void Deccelerate(float deccelRate, Vector2 targetSpeed)
        {
            float accelRate = -deccelRate;

            Accelerate(accelRate, targetSpeed);
        }

        public bool CheckIfShouldFlip(float XDirection)
        {
            if (XDirection != 0 && (int)Mathf.Sign(XDirection) != FacingDirection)
            {
                Flip();
                return true;
            }

            return false;
        }

        public void ResetFlip()
        {
            if (Mathf.Approximately(RB.transform.rotation.eulerAngles.y, 0f) && FacingDirection == -1)
            {
                FacingDirection = 1;
            }
            else if (Mathf.Approximately(RB.transform.rotation.eulerAngles.y, 180f) && FacingDirection == 1)
            {
                FacingDirection = -1;
            }
        }

        public virtual void Flip()
        {
            FacingDirection *= -1;
            RB.transform.Rotate(0.0f, 180.0f, 0.0f);
            OnFlipped?.Invoke();
        }
    }
}
