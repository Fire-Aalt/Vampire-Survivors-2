using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Game
{
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _particles = new();

        private MainModule[] _mainModules;
        private EmissionModule[] _emissionModules;
        private ShapeModule[] _shapeModules;
        private VelocityOverLifetimeModule[] _velocityOverLifetimeModules;

        private int _count;

        private Burst[][] _bursts;
        private MinMaxCurve[][] _particlesCounts;

        private void Awake()
        {
            _count = _particles.Count;
            _mainModules = new MainModule[_count];
            _emissionModules = new EmissionModule[_count];
            _shapeModules = new ShapeModule[_count];
            _velocityOverLifetimeModules = new VelocityOverLifetimeModule[_count];

            _bursts = new Burst[_count][];
            _particlesCounts = new MinMaxCurve[_count][];

            for (int i = 0; i < _count; i++)
            {
                _mainModules[i] = _particles[i].main;
                _emissionModules[i] = _particles[i].emission;
                _shapeModules[i] = _particles[i].shape;
                _velocityOverLifetimeModules[i] = _particles[i].velocityOverLifetime;

                _bursts[i] = new Burst[_emissionModules[i].burstCount];
                _emissionModules[i].GetBursts(_bursts[i]);
                _particlesCounts[i] = _bursts[i].Select(b => b.count).ToArray();
            }
        }

        public void PlayParticles(bool firstIsMain)
        {
            if (firstIsMain)
            {
                _particles[0].Play(true);
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {
                    _particles[i].Play();
                }
            }
        }

        public void StopParticles(bool firstIsMain)
        {
            if (firstIsMain)
            {
                _particles[0].Stop(true);
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {
                    _particles[i].Stop();
                }
            }
        }

        public void ChangeDuration(float newDuration, bool syncLifetime)
        {
            for (int i = 0; i < _count; i++)
            {
                _mainModules[i].duration = newDuration;
                if (syncLifetime)
                {
                    _mainModules[i].startLifetime = newDuration;
                }
            }
        }

        public void ChangeDelay(float newDelay)
        {
            for (int i = 0; i < _count; i++)
            {
                _mainModules[i].startDelay = newDelay;
            }
        }

        public void ChangeSpeedModifier(float modifier)
        {
            for (int i = 0; i < _count; i++)
            {
                _velocityOverLifetimeModules[i].speedModifier = modifier;
            }
        }

        public void ChangeEmmisionScale(float newScale)
        {
            for (int i = 0; i < _count; i++)
            {
                _shapeModules[i].scale = new Vector3(newScale, newScale);

                for (int j = 0; j < _bursts[i].Length; j++)
                {
                    MinMaxCurve newCurve = ScaleCurve(newScale, _particlesCounts[i][j]);
                    _bursts[i][j].count = newCurve;
                }
                _emissionModules[i].SetBursts(_bursts[i]);
            }
        }

        private static MinMaxCurve ScaleCurve(float newScale, MinMaxCurve defaultCurve)
        {
            return new MinMaxCurve
            {
                mode = defaultCurve.mode,

                curve = defaultCurve.curve,
                curveMax = defaultCurve.curveMax,
                curveMin = defaultCurve.curveMin,
                curveMultiplier = defaultCurve.curveMultiplier * newScale,

                constant = defaultCurve.constant * newScale,
                constantMin = defaultCurve.constantMin * newScale,
                constantMax = defaultCurve.constantMax * newScale
            };
        }
    }
}
