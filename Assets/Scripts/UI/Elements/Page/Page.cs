using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private Selectable _firstSelected;

        public CanvasGroup CanvasGroup { get; private set; }

        private float _transitionDuration;
        private CancellationTokenSource _cts;
        private SelectableRect[] _selectableRects;

        [Button]
        public void FindFirstSelected()
        {
            _firstSelected = GetComponentInChildren<Selectable>();
        }

        public void Init(float transitionDuration)
        {
            _transitionDuration = transitionDuration;
            CanvasGroup = GetComponent<CanvasGroup>();
            _selectableRects = GetComponentsInChildren<SelectableRect>(true);
        }

        public void Appear()
        {
            gameObject.SetActive(true);
            if (_firstSelected)
            {
                SetFirstSelected().Forget();
            }

            _cts?.Cancel();
            LerpAlpha(1f).Forget();
        }

        public void Disappear()
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            _cts?.Cancel();
            LerpAlpha(0f).Forget();
        }

        public async UniTaskVoid SetFirstSelected()
        {
            await UniTask.WaitUntil(() => CanvasGroup.interactable, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }

        private async UniTaskVoid LerpAlpha(float targetAlpha)
        {
            _cts = new CancellationTokenSource();
            float elapsedTime = 0f;
            float startAlpha = CanvasGroup.alpha;

            foreach (var selectable in _selectableRects)
            {
                if (targetAlpha == 1f)
                    selectable.Show(_transitionDuration);
                else if (targetAlpha == 0f)
                    selectable.Hide(_transitionDuration);
            }

            while (elapsedTime < _transitionDuration)
            {
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / _transitionDuration);
                CanvasGroup.alpha = newAlpha;

                elapsedTime += Time.unscaledDeltaTime;
                await UniTask.Yield(_cts.Token);
            }

            CanvasGroup.alpha = targetAlpha;

            if (targetAlpha == 0f)
            {
                gameObject.SetActive(false);
            }
            else
            {
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            }
        }
    }
}
