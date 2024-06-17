using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class GeneralSettingsUI : PageUI
    {
        [Header("References")]
        [SerializeField] private OptionSelection _infoModeSelection;

        private GeneralSettings _settings;

        private Dictionary<string, AbilityUpgradeInfoMode> _infoModeOptions = new() { 
            { "Кратко", AbilityUpgradeInfoMode.OnlyChange },
            { "Детально", AbilityUpgradeInfoMode.Full },
        };

        protected override void InitUI()
        {
            _settings = SettingsManager.Current.GeneralSettings;

            var infoMode = _settings.AbilityUpgradeInfoMode switch
            {
                AbilityUpgradeInfoMode.OnlyChange => 0,
                AbilityUpgradeInfoMode.Full => 1,
                _ => 0,
            };

            _infoModeSelection.InitializeNewOptions(_infoModeOptions.Keys.ToArray(), infoMode);
        }

        public void SetAbilityUpgradeInfoMode(int index)
        {
            _settings.SetAbilityUpgradeInfoMode(_infoModeOptions.Values.ToArray()[index]);
        }
    }
}
