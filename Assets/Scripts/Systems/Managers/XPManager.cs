using System;
using UnityEngine;

namespace Game
{
    public class XPManager : MonoBehaviour
    {
        public static event Action<int> OnLevelChanged;
        public static event Action<float> OnLevelPercentChanged;

        public int XPLevel { get; private set; }
        public long XPAmount { get; private set; }

        private long _levelXP, _nextLevelXP;

        private void Start()
        {
            CalculateNextLevelXP();
        }

        private void HandleEnemyDeath(Enemy enemy)
        {
            _levelXP += enemy.Data.xpAmount;

            bool levelChanged = false;
            while (_levelXP >= _nextLevelXP)
            {
                _levelXP -= _nextLevelXP;

                XPLevel++;
                levelChanged = true;
                CalculateNextLevelXP();
            }

            if (levelChanged)
            {
                OnLevelChanged?.Invoke(XPLevel);
            }
            OnLevelPercentChanged?.Invoke(_levelXP / (float)_nextLevelXP);
        }

        private void CalculateNextLevelXP()
        {
            int nextLevel = XPLevel + 1;
            if (nextLevel == 20)
            {
                _nextLevelXP = nextLevel * 5 - 2 + 600;
            }
            else if (nextLevel == 40)
            {
                _nextLevelXP = nextLevel * 6 - 3 + 2400;
            }
            else if (nextLevel < 20)
            {
                _nextLevelXP = nextLevel * 5 - 2;
            }
            else if (nextLevel < 40)
            {
                _nextLevelXP = nextLevel * 6 - 3;
            }
            else if (nextLevel > 40)
            {
                _nextLevelXP = nextLevel * 8 - 4;
            }
        }

        private void OnEnable()
        {
            Enemy.OnDeath += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            Enemy.OnDeath -= HandleEnemyDeath;
        }
    }
}
