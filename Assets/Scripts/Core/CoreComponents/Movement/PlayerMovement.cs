using MoreMountains.Tools;
using System;
using UnityEngine;

namespace Game.CoreSystem
{
    public class PlayerMovement : Movement
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _decceleration;

        public event Action<int> OnPlayerFlipped;
        public static int PlayerFacingDirection { get; private set; }

        public void Move(Vector2 input, float lerp, float speedMultiplier)
        {
            Vector2 targetSpeed = input * _speed * speedMultiplier;

            targetSpeed = Vector2.Lerp(CurrentVelocity, targetSpeed * VelocityMultiplier, lerp);

            if ((targetSpeed - CurrentVelocity).magnitude > 0.01f)
            {
                Accelerate(_acceleration, targetSpeed);
            }
            else
            {
                Deccelerate(_decceleration, targetSpeed);
            }
        }

        public override void Flip()
        {
            base.Flip();

            PlayerFacingDirection = FacingDirection;
            OnPlayerFlipped?.Invoke(FacingDirection);
        }
    }
}