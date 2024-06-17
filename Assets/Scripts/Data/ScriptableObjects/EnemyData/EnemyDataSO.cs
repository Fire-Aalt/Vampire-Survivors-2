using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game
{
    public class EnemyDataSO : EntityDataSO
    {
        public int xpAmount;
        public EnemyDataStats stats;
    }

    [Serializable]
    public class EnemyDataStats
    {
        public int health;
        public int defence;
        public int damage;
    }
}
