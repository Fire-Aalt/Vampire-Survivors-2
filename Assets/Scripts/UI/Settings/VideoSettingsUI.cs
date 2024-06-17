using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VideoSettingsUI : PageUI
    {
        [Header("References")]
        [SerializeField] private OptionSelection _resolutionSelection;
        [SerializeField] private OptionSelection _fullscreenSelection;
        [SerializeField] private OptionSelection _vsyncSelection;
        [SerializeField] private OptionSelection _qualitySelection;

        private VideoSettings _settings;

        private readonly string[] _toggleOptions = new[] { "Выкл", "Вкл" };
        private string[] _resolutionOptions;
        private string[] _qualityOptions;

        protected override void InitUI()
        {
            _settings = SettingsManager.Current.VideoSettings;

            _resolutionOptions = ParseResolutions(_settings.Resolutions);
            _qualityOptions = _settings.Qualities;

            _resolutionSelection.InitializeNewOptions(_resolutionOptions, _settings.ResolutionIndex);
            _fullscreenSelection.InitializeNewOptions(_toggleOptions, _settings.IsFullscreen ? 1 : 0);
            _vsyncSelection.InitializeNewOptions(_toggleOptions, _settings.VSyncIndex);
            //_qualitySelection.InitializeNewOptions(_qualityOptions, _settings.QualityIndex);
        }

        private string[] ParseResolutions(Resolution[] resolutions)
        {
            List<string> options = new();
            for (int i = 0; i < resolutions.Length; i++)
            {
                string resolutionOption = $"{resolutions[i].width}x{resolutions[i].height} @ {Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value)}Hz";
                options.Add(resolutionOption);
            }
            return options.ToArray();
        }

        public void SetResolution(int resolutionIndex) => _settings.SetResolution(resolutionIndex);
        public void SetFullscreen(int fullscreenIndex) => _settings.SetFullscreen(fullscreenIndex == 1);
        public void SetVSync(int vSyncIndex) => _settings.SetVSync(vSyncIndex);
        public void SetQuality(int qualityIndex) => _settings.SetQuality(qualityIndex);
    }
}
