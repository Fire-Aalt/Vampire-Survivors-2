using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class AbilityUpgradesManager : MonoBehaviour
    {
        public static readonly int MaxSlots = 3;
        public static event Action<List<AbilitySO>> OnRollAbilities;
        public static event Action<List<AbilitySO>> OnRollUpgrades;

        public List<AbilitySO> abilitiesRoster = new();
        public List<int> newAbilityLevels = new();

        [Title("Debug")]
        [ReadOnly] public List<AbilitySO> activeAbilities = new();

        private int _currentLevel = 0;
        private bool _choiceMade;

        private void Start()
        {
            foreach (var ability in abilitiesRoster)
            {
                ability.InitializeRuntime();
            }
        }

        private void UpgradeLoop(int newLevel)
        {
            UpgradeLoopTask(newLevel).Forget();
        }

        private async UniTaskVoid UpgradeLoopTask(int newLevel)
        {
            for (int level = _currentLevel; level < newLevel; level++)
            {
                if (newAbilityLevels.Contains(level))
                {
                    var possibleAbilities = abilitiesRoster.Except(activeAbilities).ToList();
                    if (possibleAbilities.Count > 0)
                    {
                        RollAbilities(possibleAbilities);
                    }
                    else
                    {
                        RollUpgrades();
                    }
                }
                else
                {
                    RollUpgrades();
                }

                await UniTask.WaitUntil(() => _choiceMade, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
                _choiceMade = false;
            }
            _currentLevel = newLevel;
        }

        private void UpgradeChoice(AbilitySO ability)
        {
            if (!activeAbilities.Contains(ability))
            {
                AbilityController.Instance.InitializeAbility(ability);
            }
            else
            {
                ability.UpgradeAbility();
            }
            _choiceMade = true;
        }

        private void RollAbilities(List<AbilitySO> possibleAbilities)
        {
            List<AbilitySO> chosen = ChooseRandomAbilities(possibleAbilities);
            OnRollAbilities?.Invoke(chosen);
        }

        private void RollUpgrades()
        {
            List<AbilitySO> possibleUpgrades = new();
            foreach (var active in activeAbilities)
            {
                if (active.UpgradeLevel != active.upgradeInfos.Count)
                {
                    possibleUpgrades.Add(active);
                }
            }

            List<AbilitySO> chosen = ChooseRandomAbilities(possibleUpgrades);
            if (chosen.Count > 0)
            {
                OnRollUpgrades?.Invoke(chosen);
            }
        }

        private List<AbilitySO> ChooseRandomAbilities(List<AbilitySO> possibleAbilities)
        {
            List<AbilitySO> chosen = new();
            while (chosen.Count < MaxSlots && chosen.Count < possibleAbilities.Count)
            {
                int id = Random.Range(0, possibleAbilities.Count);
                AbilitySO ability = possibleAbilities[id];

                if (!chosen.Contains(ability))
                {
                    chosen.Add(ability);
                }
            }

            return chosen;
        }

        private void AddActiveAbility(AbilitySO ability)
        {
            activeAbilities.Add(ability);
        }

        private void OnEnable()
        {
            XPBarUI.OnNewLevelUI += UpgradeLoop;
            AbilityController.OnAbilityInitialized += AddActiveAbility;
            AbilityUpgradesUIManager.OnAbilitySelected += UpgradeChoice;
        }

        private void OnDisable()
        {
            XPBarUI.OnNewLevelUI -= UpgradeLoop;
            AbilityController.OnAbilityInitialized -= AddActiveAbility;
            AbilityUpgradesUIManager.OnAbilitySelected -= UpgradeChoice;
        }
    }
}
