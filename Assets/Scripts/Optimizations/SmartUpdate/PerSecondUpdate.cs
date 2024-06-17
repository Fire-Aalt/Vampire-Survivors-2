using System;
using UnityEngine;

namespace Game
{
    public class PerSecondUpdate : MonoBehaviour
    {
        public static event Action OnTwentyTimes;
        public static event Action OnTenTimes;
        public static event Action OnFiveTimes;
        public static event Action OnTwoTimes;
        public static event Action OnOneTime;

        private float[] _timers;
        private float[] _intervals;

        private void Start()
        {
            _timers = new float[5];
            _intervals = new float[5]
            {
                0.05f,
                0.1f,
                0.2f,
                0.5f,
                1f
            };
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _timers.Length; i++)
            {
                _timers[i] += deltaTime;

                switch (i)
                {
                    case 0:
                        TryInvokeUpdate(i, OnTwentyTimes);
                        break;
                    case 1:
                        TryInvokeUpdate(i, OnTenTimes);
                        break;
                    case 2:
                        TryInvokeUpdate(i, OnFiveTimes);
                        break;
                    case 3:
                        TryInvokeUpdate(i, OnTwoTimes);
                        break;
                    case 4:
                        TryInvokeUpdate(i, OnOneTime);
                        break;
                }
            }
        }
            
        private void TryInvokeUpdate(int i, Action UpdateLoop)
        {
            if (_timers[i] >= _intervals[i])
            {
                UpdateLoop?.Invoke();
                _timers[i] = 0f;
            }
        }
    }
}
