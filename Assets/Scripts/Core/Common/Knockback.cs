using Cysharp.Threading.Tasks;
using Game.CoreSystem;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class Knockback
    {
        public float _knockbackDuration;

        public bool IsKnockback { get; protected set; }

        public Movement movement;
        public CancellationTokenSource knockbackCTS;

        public Knockback(Movement movement, float knockbackDuration = 0.2f)
        {
            this.movement = movement;
            _knockbackDuration = knockbackDuration;
        }

        /// <summary>
        /// If duration == 0f then duration will be reset to default value
        /// </summary>
        public void ApplyKnockback(Vector2 angle, float strength, float duration = 0f, float terminateVelocity = 0f, bool ignoreGravity = false)
        {
            if (duration == 0f)
                duration = _knockbackDuration;

            knockbackCTS?.Cancel();

            angle.Normalize();
            float decceleration = strength / duration;

            movement.SetVelocity(strength, angle);

            ResetKnockback(decceleration, duration, terminateVelocity, ignoreGravity).Forget();
        }

        private async UniTaskVoid ResetKnockback(float decceleration, float duration, float terminateVelocity, bool ignoreGravity)
        {
            knockbackCTS = new CancellationTokenSource();
            ChangeState(true);

            float elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;

                var time = duration - elapsedTime;
                var velocity = decceleration * time / 2;
                if (velocity < terminateVelocity)
                    break;

                Vector2 angle = movement.CurrentVelocity.normalized;
                if (movement != null)
                {
                    if (!ignoreGravity)
                        movement.SetVelocityX(velocity * angle.x, true);
                    else
                        movement.SetVelocity(velocity, angle, true);
                }
                
                await UniTask.WaitForFixedUpdate(knockbackCTS.Token);
            }

            ChangeState(false);
        }

        private void ChangeState(bool isKnockback)
        {
            IsKnockback = isKnockback;
            movement.CanSetVelocity = !isKnockback;
        }
    }
}
