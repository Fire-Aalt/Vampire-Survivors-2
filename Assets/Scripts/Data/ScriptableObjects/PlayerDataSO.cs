using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Player Data")]
    public class PlayerDataSO : EntityDataSO
    {
        public List<PlayerAbilityData> startingAbilities = new();
    }

    [Serializable]
    public class PlayerAbilityData
    {
        public AbilitySO ability;
        [OnValueChanged("Validate")] public int abilityLevel;

        public void Validate()
        {
            int clamped = Mathf.Clamp(abilityLevel, 0, ability.upgradeInfos.Count);
            if (clamped != abilityLevel)
            {
                abilityLevel = clamped;
            }
        }
    }
}
