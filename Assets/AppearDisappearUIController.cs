using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AppearDisappearUIController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _target;
        public MMF_Player appearPlayer;
        public MMF_Player disappearPlayer;

        public void PlayAppear(PauseStates state)
        {
            appearPlayer.PlayFeedbacks();
            PauseManager.UpdatePauseState(state);
        }

        public void PlayDisappear(PauseStates state)
        {
            disappearPlayer.PlayFeedbacks();
            PauseManager.UpdatePauseState(state);
        }

        [Button]
        public void GetTargetInParent()
        {
            _target = GetComponentInParent<CanvasGroup>();
        }
    }
}
