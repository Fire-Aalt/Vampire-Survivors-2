using Game.CoreSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HPBarUI : MonoBehaviour
    {
        [SerializeField] private float _updateDuration;
        [SerializeField] private AnimationCurve _updateCurve;
        [SerializeField] private Slider _slider;

        private float _elapsedTime;
        private float _startValue, _currentValue, _endValue;

        public bool IsUpdating { get; private set; }

        private void Start()
        {
            _currentValue = 1f;
        }

        private void Update()
        {
            if (IsUpdating)
            {
                if (_elapsedTime < _updateDuration)
                {
                    _elapsedTime += Time.deltaTime;

                    _currentValue = Mathf.Lerp(_startValue, _endValue, _updateCurve.Evaluate(_elapsedTime / _updateDuration));
                    _slider.value = _currentValue;
                }
                else
                {
                    IsUpdating = false;
                }
            }
        }

        private void HandleHealthChanged(int curHP, int maxHP)
        {
            _startValue = _currentValue;
            _endValue = curHP / (float)maxHP;
            _elapsedTime = 0f;
            IsUpdating = true;
        }

        private void OnEnable()
        {
            PlayerStats.OnHealthChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            PlayerStats.OnHealthChanged -= HandleHealthChanged;
        }
    }
}
