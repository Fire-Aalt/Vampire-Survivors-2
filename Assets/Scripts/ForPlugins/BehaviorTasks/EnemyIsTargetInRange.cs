using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyIsTargetInRange : EnemyConditional
    {
        public Transform origin;
        public float activationRange = 10;
        public Vector2 activationRefreshDuration = new(3f, 3f);

        private float _activationRangeSqr;
        private Timer _activationRefreshTimer;

        private bool _canActivate;

        public override void OnAwake()
        {
            base.OnAwake();

            origin = transform;
            _activationRangeSqr = activationRange * activationRange;
            _activationRefreshTimer = new Timer();
            _activationRefreshTimer.OnTimerDone += () => _canActivate = true;
            _canActivate = true;
        }

        public override TaskStatus OnUpdate()
        {
            _activationRefreshTimer.Tick();

            if (Enemy.Target == null)
            {
                return TaskStatus.Failure;
            }

            Vector3 targetPos = Enemy.Target.position;
            if (_canActivate && (origin.position - targetPos).sqrMagnitude < _activationRangeSqr)
            {
                _canActivate = false;
                _activationRefreshTimer.StartTimer(Random.Range(activationRefreshDuration.x, activationRefreshDuration.y));
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (origin != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(origin.position, activationRange);
            }
        }
    }
}
