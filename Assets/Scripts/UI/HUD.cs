using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class HUD : MonoBehaviour, IGameDataPersistence
    {
        [SerializeField] private Slider _healthBar;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private string _levelFormat;

        public static float ScoreTime { get; private set; }
        private float _displayedSecondsLastUpdate;

        public void LoadData(GameData data)
        {
            ScoreTime = 0f;
        }

        public void SaveData(GameData data)
        {
            data.scoreTime = Mathf.Max(ScoreTime, data.scoreTime);
        }

        private void Start()
        {
            UpdateLevelText(0);
        }

        private void Update()
        {
            ScoreTime += Time.deltaTime;
            BreakDownTime(ScoreTime, out int seconds, out int minutes);
            if (_displayedSecondsLastUpdate != seconds)
            {
                _timeText.text = FormatTime(ScoreTime);
                _displayedSecondsLastUpdate = seconds;
            }
        }

        public static string FormatTime(float time)
        {
            BreakDownTime(time, out int seconds, out int minutes);

            string res = minutes + ":";
            if (seconds < 10)
            {
                res += "0";
            }
            res += seconds;

            return res;
        }

        private static void BreakDownTime(float time, out int seconds, out int minutes)
        {
            seconds = Mathf.FloorToInt(time);
            minutes = seconds / 60;
            seconds %= 60;
        }

        public void UpdateHealthBar(int currentHealth)
        {
            _healthBar.value = currentHealth;
        }

        public void UpdateLevelText(int level)
        {
            _levelText.text = _levelFormat.Replace("{lvl}", level.ToString());
        }

        private void OnEnable()
        {
            XPBarUI.OnNewLevelUI += UpdateLevelText;
        }

        private void OnDisable()
        {
            XPBarUI.OnNewLevelUI -= UpdateLevelText;
        }
    }
}

