using RenderDream.GameEssentials;

namespace Game
{
    public class SettingsData : SettingsDataModel
    {
        public int resolutionIndex;
        public bool isFullscreen;
        public int vSyncIndex;
        public int qualityIndex;
        public int masterVolumeLevel;
        public int musicVolumeLevel;
        public int sfxVolumeLevel;
        public AbilityUpgradeInfoMode abilityUpgradeInfoMode;

        public SettingsData()
        {
            resolutionIndex = -1;
            isFullscreen = true;
            vSyncIndex = -1;
            qualityIndex = -1;
            masterVolumeLevel = -1;
            musicVolumeLevel = -1;
            sfxVolumeLevel = -1;
            abilityUpgradeInfoMode = AbilityUpgradeInfoMode.OnlyChange;
        }
    }
}
