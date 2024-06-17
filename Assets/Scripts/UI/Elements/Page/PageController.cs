using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class PageController : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private ControllerMode _controllerMode;
        [SerializeField] private bool _showFirstPageOnStart;

        [Title("Pages Setup")]
        [SerializeField] private float _transitionDuration = 0.2f;

        [Title("Pages")]
        [SerializeField] private Page _backgroundPage;
        [SerializeField] private List<Page> _pages = new();

        public bool IsActiveController { get; set; }
        public Page PreviousPage { get; private set; }
        public Page CurrentPage { get; private set; }

        [Button]
        public void FindRelativePages()
        {
            _pages.Clear();
            RecursiveSearch(transform);
        }

        private void RecursiveSearch(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.TryGetComponent(out Page page) && page != _backgroundPage)
                {
                    _pages.Add(page);
                }
                else if (!child.TryGetComponent(out PageController _))
                {
                    RecursiveSearch(child);
                }
            }
        }

        private void Start()
        {
            CurrentPage = _pages[0];

            if (_backgroundPage != null)
            {
                _backgroundPage.Init(_transitionDuration);
                ChangePageState(_backgroundPage, isActive: false);
            }
            foreach (var page in _pages)
            {
                page.Init(_transitionDuration);
                if (page == CurrentPage && _showFirstPageOnStart)
                {
                    ChangePageState(page, isActive: true);
                    _backgroundPage?.Appear();
                }
                else
                {
                    ChangePageState(page, isActive: false);
                }
            }
        }

        public void TransitionToFirstPage()
        {
            if (PreviousPage != null)
            {
                CurrentPage.Disappear();
                PreviousPage = CurrentPage;
            }

            CurrentPage = _pages[0];
            _backgroundPage?.Appear();
            CurrentPage.Appear();
            IsActiveController = true;
        }

        public void TransitionBack()
        {
            CurrentPage.Disappear();
            PreviousPage.Appear();
            (PreviousPage, CurrentPage) = (CurrentPage, PreviousPage);
        }

        public void TransitionToPage(Page newPage)
        {
            if (newPage != CurrentPage)
            {
                CurrentPage.Disappear();
                PreviousPage = CurrentPage;
            }
            newPage.Appear();
            CurrentPage = newPage;
        }

        public void TransitionToController(PageController newController)
        {
            _backgroundPage?.Disappear();
            CurrentPage.Disappear();
            IsActiveController = false;
            newController.TransitionToFirstPage();
        }

        public void TransitionOut()
        {
            if (IsActiveController)
            {
                _backgroundPage?.Disappear();
                CurrentPage.Disappear();
            }
            CurrentPage = _pages[0];
            IsActiveController = false;
        }

        private void ChangePageState(Page page, bool isActive)
        {
            page.CanvasGroup.alpha = isActive ? 1f : 0f;
            page.CanvasGroup.interactable = isActive;
            page.CanvasGroup.blocksRaycasts = isActive;
            page.gameObject.SetActive(isActive);

            if (isActive)
            {
                page.SetFirstSelected().Forget();
            }
        }

        public enum ControllerMode
        {
            LerpAlpha,
            MoveInCycles
        }
    }
}
