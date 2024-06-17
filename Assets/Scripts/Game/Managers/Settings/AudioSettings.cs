using MoreMountains.Tools;
using System.Diagnostics;

namespace Game
{
    public class AudioSettings
    {
        /// <summary>
        /// Always n - 1 (1 being level zero)
        /// </summary>
        public const int VOLUME_LEVELS = 10;

        public int MasterVolumeLevel { get; private set; }
        public int MusicVolumeLevel { get; private set; }
        public int SfxVolumeLevel { get; private set; }

        public void ApplySettings(int masterVolumeLevel, int musicVolumeLevel, int sfxVolumeLevel)
        {
            ParseValues(masterVolumeLevel, musicVolumeLevel, sfxVolumeLevel);

            SetMasterVolume(MasterVolumeLevel);
            SetMusicVolume(MusicVolumeLevel);
            SetSFXVolume(SfxVolumeLevel);
        }

        private void ParseValues(int masterVolumeLevel, int musicVolumeLevel, int sfxVolumeLevel)
        {
            if (masterVolumeLevel == -1)
            {
                masterVolumeLevel = VOLUME_LEVELS;
            }
            MasterVolumeLevel = masterVolumeLevel;
            if (musicVolumeLevel == -1)
            {
                musicVolumeLevel = VOLUME_LEVELS;
            }
            MusicVolumeLevel = musicVolumeLevel;
            if (sfxVolumeLevel == -1)
            {
                sfxVolumeLevel = VOLUME_LEVELS;
            }
            SfxVolumeLevel = sfxVolumeLevel;
        }

        public void SetMasterVolume(int volumeLevel)
        {
            SoundManager.Advance.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Master, GetFloatVolume(volumeLevel));
            MasterVolumeLevel = volumeLevel;
        }

        public void SetMusicVolume(int volumeLevel)
        {
            SoundManager.Advance.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music, GetFloatVolume(volumeLevel));
            MusicVolumeLevel = volumeLevel;
        }

        public void SetSFXVolume(int volumeLevel)
        {
            SoundManager.Advance.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Sfx, GetFloatVolume(volumeLevel));
            SfxVolumeLevel = volumeLevel;
        }

        public float GetFloatVolume(int volumeLevel) => volumeLevel / (float)VOLUME_LEVELS;
    }
}
