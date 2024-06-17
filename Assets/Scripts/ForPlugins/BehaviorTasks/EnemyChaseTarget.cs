using BehaviorDesigner.Runtime.Tasks;
using Game.CoreSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class EnemyChaseTarget : EnemyAction
    {
        public float speed = 1f;
        public bool useChaseDuration;
        [ShowIf("useChaseDuration")] public Vector2 randomChaseDuration;

        private Movement _movement;
        private Timer _chaseTimer;

        private bool _inChase;

        public override void OnAwake()
        {
            base.OnAwake();
            _movement = Enemy.Movement;
            _chaseTimer = new Timer();
            _chaseTimer.OnTimerDone += () => _inChase = false;
        }

        public override void OnStart()
        {
            base.OnStart();
            if (useChaseDuration)
            {
                float chaseDuration = Random.Range(randomChaseDuration.x, randomChaseDuration.y);
                _chaseTimer.StartTimer(chaseDuration);
            }
            _inChase = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (useChaseDuration)
            {
                _chaseTimer.Tick();
            }

            if (_inChase)
                return TaskStatus.Running;
            else
            {
                _movement.SetVelocityZero();
                return TaskStatus.Success;
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            var direction = Enemy.Target.position - transform.position;
            if (direction.sqrMagnitude > 0.25f)
            {
                direction.Normalize();

                _movement.SetVelocity(speed, direction);
                _movement.CheckIfShouldFlip(direction.x);
            }
        }
    }
}
