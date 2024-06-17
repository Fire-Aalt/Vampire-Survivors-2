using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Game
{
    public class SoundManager : MMSoundManager
    {
        /// <summary>
        /// Advance stands for SoundManager, while Current and Instance stand for MMSoundManager
        /// </summary>
        public static SoundManager Advance { get; private set; }

        public readonly Dictionary<MMSoundManagerTracks, float> TrackDefaultVolume = new();
        public readonly Dictionary<MMSoundManagerTracks, float> FadedTrackFactor = new();

        protected override void Awake()
        {
            base.Awake();
            Advance = this;
        }

        protected override void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
        {
            if (loadSceneMode == LoadSceneMode.Single)
            {
                FreeAllSoundsButPersistent();
            }
        }

        public override void PlayTrack(MMSoundManagerTracks track)
        {
            foreach (MMSoundManagerSound sound in _sounds)
            {
                if (sound.Track == track && sound.Source.gameObject.activeSelf)
                {
                    sound.Source.Play();
                }
            }
        }

        public override void SetTrackVolume(MMSoundManagerTracks track, float volume)
        {
            
            if (TrackDefaultVolume.ContainsKey(track))
            {
                TrackDefaultVolume[track] = volume;
            }
            else
            {
                TrackDefaultVolume.Add(track, volume);
            }

            if (FadedTrackFactor.ContainsKey(track))
            {
                volume = FadedTrackFactor[track] * volume;
            }

            base.SetTrackVolume(track, volume);
        }

        protected override IEnumerator FadeTrackCoroutine(MMSoundManagerTracks track, float duration, float initialVolume, float finalVolume, MMTweenType tweenType)
        {
            if (FadedTrackFactor.ContainsKey(track))
            {
                FadedTrackFactor[track] = finalVolume;
            }
            else
            {
                FadedTrackFactor.Add(track, finalVolume);
            }

            finalVolume = TrackDefaultVolume[track] * finalVolume;

            yield return base.FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType);

            if (finalVolume == TrackDefaultVolume[track])
            {
                FadedTrackFactor.Remove(track);
            }
        }
    }
}
