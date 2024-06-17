using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AbilityUpgradesUIManager : MonoBehaviour
    {
        public static AbilityUpgradeInfoMode UpgradeInfoMode { get => _upgradeInfoMode; set
            {
                _upgradeInfoMode = value;
                OnUpgradeInfoModeChanged?.Invoke(value);
            }
        }
        private static AbilityUpgradeInfoMode _upgradeInfoMode;

        public static event Action<AbilityUpgradeInfoMode> OnUpgradeInfoModeChanged;
        public static event Action<AbilitySO> OnAbilitySelected;

        [Title("Upgrade Slots")]
        [SerializeField] private Transform _slotsHolder;
        [SerializeField] private UpgradeInfoUI _upgradeInfoPrefab;

        [Title("Feedbacks")]
        [SerializeField] private AppearDisappearUIController _appearDisappearController;
        [SerializeField] private float _transitionDuration;

        private List<UpgradeInfoUI> _upgradeInfos;
        private List<AbilitySO> _abilities;

        private void Start()
        {
            _upgradeInfos = new List<UpgradeInfoUI>();
            for (int i = 0; i < AbilityUpgradesManager.MaxSlots; i++)
            {
                var info = Instantiate(_upgradeInfoPrefab);
                info.transform.SetParent(_slotsHolder, false);
                info.Initialize(i);
                info.OnSlotClicked += ProcessSlotClicked;
                _upgradeInfos.Add(info);
            }
        }

        public void DisplayNewAbilities(List<AbilitySO> abilities)
        {
            Appear(abilities);
            for (int i = 0; i < abilities.Count; i++)
            {
                _upgradeInfos[i].ShowNewAbility(abilities[i]);
            }
        }

        public void DisplayAbilityUpgrades(List<AbilitySO> abilities)
        {
            Appear(abilities);
            for (int i = 0; i < abilities.Count; i++)
            {
                _upgradeInfos[i].ShowAbilityUpgrade(abilities[i]);
            }
        }

        private void Appear(List<AbilitySO> abilities)
        {
            _abilities = abilities;
            _appearDisappearController.PlayAppear(PauseStates.Inventory);
            for (int i = 0; i < abilities.Count; i++)
            {
                _upgradeInfos[i].gameObject.SetActive(true);
                _upgradeInfos[i].Show(_transitionDuration);
            }
            for (int i = abilities.Count; i < _upgradeInfos.Count; i++)
            {
                _upgradeInfos[i].gameObject.SetActive(false);
            }
        }

        private void ProcessSlotClicked(UpgradeInfoUI info)
        {
            _appearDisappearController.PlayDisappear(PauseStates.None);
            for (int i = 0; i < _abilities.Count; i++)
            {
                _upgradeInfos[i].Hide(_transitionDuration);
            }

            OnAbilitySelected?.Invoke(_abilities[info.Id]);
        }

        private void OnEnable()
        {
            AbilityUpgradesManager.OnRollAbilities += DisplayNewAbilities;
            AbilityUpgradesManager.OnRollUpgrades += DisplayAbilityUpgrades;
        }

        private void OnDisable()
        {
            AbilityUpgradesManager.OnRollAbilities -= DisplayNewAbilities;
            AbilityUpgradesManager.OnRollUpgrades -= DisplayAbilityUpgrades;
        }
    }

    public enum AbilityUpgradeInfoMode 
    {
        Full,
        OnlyChange,
        Brief
    }
}
