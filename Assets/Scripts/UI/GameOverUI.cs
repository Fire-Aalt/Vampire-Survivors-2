using MoreMountains.Feedbacks;
using UnityEngine;
using RenderDream.GameEssentials;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private MMF_Player _animationPlayer;
        [SerializeField] private AppearDisappearUIController _appearDisappearController;

        private void ShowOverlay()
        {
            AwaitPauseMenuChange().Forget();
        }

        private async UniTaskVoid AwaitPauseMenuChange()
        {
            await UniTask.WaitUntil(() => PauseManager.State == PauseStates.None);
            _appearDisappearController.PlayAppear(PauseStates.InGamePause);
            _animationPlayer.PlayFeedbacks();
        }

        public void MainMenu()
        {
            if (!SceneLoader.IsTransitioning)
            {
                SceneLoader.Current.LoadSceneGroup(0, false, SceneTransition.TransitionInAndOut).Forget();
                PauseManager.UpdatePauseState(PauseStates.None);
            }
        }

        private void OnEnable()
        {
            Player.OnDeath += ShowOverlay;
        }

        private void OnDisable()
        {
            Player.OnDeath -= ShowOverlay;
        }
    }
}
