using System.Linq;
using UnityEngine;

namespace Game
{
    public class VideoSettings
    {
        public int ResolutionIndex { get; private set; }
        public bool IsFullscreen { get; private set; }
        public int VSyncIndex { get; private set; }
        public int QualityIndex { get; private set; }

        public Resolution[] Resolutions { get; private set; }
        public string[] Qualities { get; private set; }

        public VideoSettings()
        {
            Resolutions = GetResolutions();
            Qualities = QualitySettings.names;
        }

        public void ApplySettings(int resolutionIndex, bool isFullscreen, int vSyncIndex, int qualityIndex)
        {
            ParseValues(resolutionIndex, isFullscreen, vSyncIndex, qualityIndex);
            
            SetResolution(ResolutionIndex);
            SetFullscreen(IsFullscreen);
            SetVSync(VSyncIndex);
            SetQuality(QualityIndex);
        }

        private void ParseValues(int resolutionIndex, bool isFullscreen, int vSyncIndex, int qualityIndex)
        {
            if (resolutionIndex == -1 || resolutionIndex >= Resolutions.Length)
            {
                for (int i = 0; i < Resolutions.Length; i++)
                {
                    if (Resolutions[i].width == Screen.width && Resolutions[i].height == Screen.height)
                    {
                        resolutionIndex = i;
                        break;
                    }
                }
            }
            ResolutionIndex = resolutionIndex;
            IsFullscreen = isFullscreen;
            if (vSyncIndex == -1)
            {
                vSyncIndex = 1;
            }
            VSyncIndex = vSyncIndex;
            if (qualityIndex == -1)
            {
                qualityIndex = Qualities.Length - 1;
            }
            QualityIndex = qualityIndex;
        }

        private Resolution[] GetResolutions()
        {
            Resolution[] resolutions = Screen.resolutions;

            int _currentRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
            Resolution[] filteredResolutions = resolutions.Where(r => Mathf.RoundToInt((float)r.refreshRateRatio.value) == _currentRefreshRate).ToArray();

            if (filteredResolutions.Length > 0)
            {
                resolutions = filteredResolutions;
            }

            return resolutions;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = Resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
            ResolutionIndex = resolutionIndex;
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            IsFullscreen = isFullscreen;
        }

        public void SetVSync(int vSyncIndex)
        {
            QualitySettings.vSyncCount = vSyncIndex;
            VSyncIndex = vSyncIndex;
        }

        public void SetQuality(int qualityIndex)
        {
            //QualitySettings.SetQualityLevel(qualityIndex);
            QualityIndex = qualityIndex;
        }
    }
}
