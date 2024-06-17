using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GeneralSettings
    {
        public AbilityUpgradeInfoMode AbilityUpgradeInfoMode { get; private set; }

        public void ApplySettings(AbilityUpgradeInfoMode abilityUpgradeInfoMode)
        {
            SetAbilityUpgradeInfoMode(abilityUpgradeInfoMode);
        }

        public void SetAbilityUpgradeInfoMode(AbilityUpgradeInfoMode abilityUpgradeInfoMode)
        {
            AbilityUpgradesUIManager.UpgradeInfoMode = abilityUpgradeInfoMode;
            AbilityUpgradeInfoMode = abilityUpgradeInfoMode;
        }
    }
}
