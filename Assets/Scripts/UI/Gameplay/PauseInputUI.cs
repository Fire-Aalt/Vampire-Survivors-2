using Game.UI;
using MoreMountains.Feedbacks;
using UnityEngine;
using RenderDream.GameEssentials;

namespace Game
{
    public class PauseInputUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PageController _settingsPageController;

        [Header("MMF Players")]
        [SerializeField] private AppearDisappearUIController _appearDisappearController;

        private PageController _pageController;

        private void Awake()
        {
            _pageController = GetComponent<PageController>();
        }

        private void HandlePauseInput()
        {
            if (PauseManager.State == PauseStates.None)
            {
                Pause();
            }
            else if (PauseManager.State == PauseStates.PauseMenu)
            {
                Resume();
            }
        }

        private void Pause()
        {
            _appearDisappearController.PlayAppear(PauseStates.PauseMenu);
            _pageController.TransitionToFirstPage();
        }

        private void Resume()
        {
            _appearDisappearController.PlayDisappear(PauseStates.None);
            _pageController.TransitionOut();
            _settingsPageController.TransitionOut();
        }

        public void TransitionToSettingsMenu()
        {
            _pageController.TransitionToController(_settingsPageController);
        }


        public void Continue()
        {
            Resume();
        }

        public void ReturnToMainMenu()
        {
            if (!SceneLoader.IsTransitioning)
            {
                SceneLoader.Current.LoadSceneGroup(0, false, SceneTransition.TransitionInAndOut);
                PauseManager.UpdatePauseState(PauseStates.None);
            }
        }

        private void OnEnable()
        {
            InputManager.OnPauseInput += HandlePauseInput;
        }

        private void OnDisable()
        {
            InputManager.OnPauseInput -= HandlePauseInput;
        }
    }
}
