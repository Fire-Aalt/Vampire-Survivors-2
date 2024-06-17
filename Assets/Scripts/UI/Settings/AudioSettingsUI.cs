using UnityEngine;

namespace Game.UI
{
    public class AudioSettingsUI : PageUI
    {
        [Header("References")]
        [SerializeField] private OptionSelection _masterVolumeSelection;
        [SerializeField] private OptionSelection _musicVolumeSelection;
        [SerializeField] private OptionSelection _sfxVolumeSelection;

        private AudioSettings _settings;

        private string[] _volumeOptions;

        protected override void InitUI()
        {
            _settings = SettingsManager.Current.AudioSettings;

            AddVolumeOptions(AudioSettings.VOLUME_LEVELS);

            _masterVolumeSelection.InitializeNewOptions(_volumeOptions, _settings.MasterVolumeLevel);
            _musicVolumeSelection.InitializeNewOptions(_volumeOptions, _settings.MusicVolumeLevel);
            _sfxVolumeSelection.InitializeNewOptions(_volumeOptions, _settings.SfxVolumeLevel);
        }

        private void AddVolumeOptions(int volumeLevels)
        {
            int maxIndex = volumeLevels + 1;
            _volumeOptions = new string[maxIndex];
            for (int i = 0; i < maxIndex; i++)
            {
                _volumeOptions[i] = i.ToString();
            }
        }

        public void SetMasterVolume(int volumeIndex) => _settings.SetMasterVolume(volumeIndex);
        public void SetMusicVolume(int volumeIndex) => _settings.SetMusicVolume(volumeIndex);
        public void SetSFXVolume(int volumeIndex) => _settings.SetSFXVolume(volumeIndex);
    }
}
