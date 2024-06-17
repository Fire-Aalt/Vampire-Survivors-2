using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UpgradeStatUI : MonoBehaviour
    {
        [Title("Data")]
        [SerializeField] private StatsLocalizationSO _loc;

        [Title("References")]
        [SerializeField] private TextMeshProUGUI _baseStatText;
        [SerializeField] private TextMeshProUGUI _upgradeStatText;

        [Title("Params")]
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _negativeColor;
        [SerializeField] private string _fullUpgradeFormat;
        [SerializeField] private string _onlyUpgradeFormat;
        [SerializeField] private string _briefUpgradeFormat;

        private AbilitySO _ability;
        private StatModifierInfo _modifierInfo;
        private StatModifier _modifier;

        public void Initialize(AbilitySO ability, StatModifierInfo modifierInfo)
        {
            _ability = ability;
            _modifierInfo = modifierInfo;
            _modifier = modifierInfo.modifier;

            RedrawModifier(AbilityUpgradesUIManager.UpgradeInfoMode);
            AbilityUpgradesUIManager.OnUpgradeInfoModeChanged -= RedrawModifier;
            AbilityUpgradesUIManager.OnUpgradeInfoModeChanged += RedrawModifier;
        }

        private void RedrawModifier(AbilityUpgradeInfoMode infoMode)
        {
            string localizedName = _loc.GetLocalizedName(_modifierInfo.statName, out bool valueIsInversed);
            string baseValue = (Mathf.Round(_ability.GetStat(_modifierInfo.statName).Value * 100) / 100).ToString();
            string upgradeValue = "";

            if (_modifier.Value >= 0)
                upgradeValue = "+";
            upgradeValue += _modifier.Value;
            if (_modifier.Type != StatModType.Flat)
            {
                upgradeValue += "%";
            }

            bool isChangeNegative;
            if (_modifier.Value < 0)
                isChangeNegative = !valueIsInversed;
            else 
                isChangeNegative = valueIsInversed;

            if (isChangeNegative)
                _upgradeStatText.color = _negativeColor;
            else
                _upgradeStatText.color = _positiveColor;

            string baseText = "", upgradeText = "";
            switch (infoMode)
            {
                case AbilityUpgradeInfoMode.Full:
                    baseText = _fullUpgradeFormat.Replace("{name}", localizedName);
                    baseText = baseText.Replace("{value}", baseValue);
                    upgradeText = upgradeValue;

                    _baseStatText.gameObject.SetActive(true);
                    break;
                case AbilityUpgradeInfoMode.OnlyChange:
                    upgradeText = _onlyUpgradeFormat.Replace("{name}", localizedName);
                    upgradeText = upgradeText.Replace("{value}", upgradeValue);

                    _baseStatText.gameObject.SetActive(false);
                    break;
                case AbilityUpgradeInfoMode.Brief:
                    upgradeText = _briefUpgradeFormat.Replace("{name}", localizedName);
                    upgradeText += isChangeNegative ? '-' : '+';

                    _baseStatText.gameObject.SetActive(false);
                    break;
            }

            _baseStatText.text = baseText;
            _upgradeStatText.text = upgradeText;
        }

        private void OnDisable()
        {
            AbilityUpgradesUIManager.OnUpgradeInfoModeChanged -= RedrawModifier;
        }
    }
}
