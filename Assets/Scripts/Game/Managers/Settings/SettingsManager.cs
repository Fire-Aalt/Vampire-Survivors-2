using RenderDream.GameEssentials;
using UnityEngine;

namespace Game
{
    public class SettingsManager : Singleton<SettingsManager>, ISettingsDataPersistence
    {
        public GeneralSettings GeneralSettings { get; private set; }
        public VideoSettings VideoSettings { get; private set; }
        public AudioSettings AudioSettings { get; private set; }

        private SettingsData _loadedData;

        public void LoadData(SettingsData data)
        {
            _loadedData = data;
        }

        public void SaveData(SettingsData data)
        {
            // General
            data.abilityUpgradeInfoMode = GeneralSettings.AbilityUpgradeInfoMode;
            // Video
            data.resolutionIndex = VideoSettings.ResolutionIndex;
            data.isFullscreen = VideoSettings.IsFullscreen;
            data.vSyncIndex = VideoSettings.VSyncIndex;
            data.qualityIndex = VideoSettings.QualityIndex;
            // Audio
            data.masterVolumeLevel = AudioSettings.MasterVolumeLevel;
            data.musicVolumeLevel = AudioSettings.MusicVolumeLevel;
            data.sfxVolumeLevel = AudioSettings.SfxVolumeLevel;
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (_loadedData == null)
            {
                Debug.LogError("SettingsData is Missing");
                return;
            }

            GeneralSettings = new GeneralSettings();
            VideoSettings = new VideoSettings();
            AudioSettings = new AudioSettings();
            GeneralSettings.ApplySettings(_loadedData.abilityUpgradeInfoMode);
            VideoSettings.ApplySettings(_loadedData.resolutionIndex, _loadedData.isFullscreen,
                _loadedData.vSyncIndex, _loadedData.qualityIndex);
            AudioSettings.ApplySettings(_loadedData.masterVolumeLevel, _loadedData.musicVolumeLevel,
                _loadedData.sfxVolumeLevel);
        }
    }
}
