using Game.CoreSystem;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Game
{
    public class TransformLean : CoreComponent
    {
        [SerializeField] private Transform _target;
        public float maxAffectedVelocity;
        public float maxLeanInAngles;
        public float leanDuration;
        public AnimationCurve leanCurve;

        private Movement _movement;
        private float _currentLean;
        private int _dirMultiplier = -1;

        private bool _flipped;

        protected override void Start()
        {
            base.Start();

            _movement = core.GetCoreComponent<Movement>();
            _movement.OnFlipped += HandleFlipped;
        }

        private void HandleFlipped()
        {
            _dirMultiplier *= -1;
            _flipped = true;
            ApplyLean();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            ApplyLean();
        }

        private void ApplyLean()
        {
            float xVelocity = _movement.CurrentVelocity.x;
            int xDir = (int)Mathf.Sign(_movement.CurrentVelocity.x) * _dirMultiplier;
            Vector3 euler = _target.transform.rotation.eulerAngles;

            float xVelocityNorm = Mathf.Abs(xVelocity);
            float xTransition = xVelocityNorm / maxAffectedVelocity;

            float xCurrentTransition = _currentLean / maxLeanInAngles;
            float xTargetTransition = xTransition * xDir;
            float xTargetEvaluatedLean = leanCurve.Evaluate(xTransition) * maxLeanInAngles * xDir;

            float durationFactor = Mathf.Max(xCurrentTransition, xTargetTransition) - Mathf.Min(xCurrentTransition, xTargetTransition);
            float duration = durationFactor * leanDuration;

            if (_flipped)
            {
                _currentLean = xTargetEvaluatedLean;
                _flipped = false;
            }
            else
            {
                _currentLean = Mathf.Lerp(_currentLean, xTargetEvaluatedLean, Time.deltaTime / duration);
            }

            if (float.IsNaN(_currentLean)) _currentLean = 0;
            _target.transform.rotation = Quaternion.Euler(euler.x, euler.y, _currentLean);
        }
    }
}
