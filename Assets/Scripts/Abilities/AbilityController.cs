using RenderDream.GameEssentials;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class AbilityController : Singleton<AbilityController>
    {
        public static event Action<AbilitySO> OnAbilityInitialized;

        [field: SerializeField] public PlayerDataSO Data { get; private set; }
        public bool AutiomaticWeapons = true;

        [ReadOnly] public List<Ability> activeAbilities = new();

        private List<Ability> _abilitiesList;

        private void Start()
        {
            activeAbilities.Clear();
            _abilitiesList = GetComponentsInChildren<Ability>().ToList();
            foreach (var abilityData in Data.startingAbilities)
            {
                InitializeAbility(abilityData.ability);
            }
        }

        public void InitializeAbility(AbilitySO abilityData)
        {
            var ability = _abilitiesList.FirstOrDefault(a => a.Data == abilityData);

            if (ability == null)
            {
                Debug.LogError($"{abilityData} does not have its corresponding AbilityPrefab");
            }
            else
            {
                ability.SetController(this);
                ability.Initialize();
                activeAbilities.Add(ability);
                OnAbilityInitialized?.Invoke(ability.Data);
            }
        }

        public void LogicUpdate()
        {
            foreach (var ability in activeAbilities)
            {
                ability.LogicUpdate();
            }
        }

        public void SwitchFireMode()
        {
            AutiomaticWeapons = !AutiomaticWeapons;
        }
    }
}
