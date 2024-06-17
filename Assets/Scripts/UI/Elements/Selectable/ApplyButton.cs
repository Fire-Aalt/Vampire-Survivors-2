using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ApplyButton : ElementButton
    {
        private CanvasGroup _canvasGroup;
        private CancellationTokenSource _alphaCTS;

        public bool IsInPressedTransition { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void Appear()
        {
            _alphaCTS?.Cancel();
            LerpButtonAlpha(true).Forget();
            SelectableRect.Select();
            SelectableRect.Show(data.duration);
        }

        public void Disappear()
        {
            _alphaCTS?.Cancel();
            LerpButtonAlpha(false).Forget();
            SelectableRect.Deselect();
            SelectableRect.Hide(data.duration);
        }

        public override void Press()
        {
            base.Press();
            _alphaCTS?.Cancel();
            LerpButtonAlpha(false).Forget();
            SelectableRect.Deselect();
            SelectableRect.Hide(data.duration);
            IsInPressedTransition = true;
        }

        private async UniTaskVoid LerpButtonAlpha(bool startTransition)
        {
            _alphaCTS = new();
            float startAlpha = _canvasGroup.alpha;
            float endAlpha;

            transform.localScale = _startScale;
            if (startTransition)
            {
                endAlpha = 1f;
            }
            else
            {
                endAlpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
            float duration = data.duration;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                float alpha = Mathf.Lerp(startAlpha, endAlpha, (elapsedTime / duration));
                _canvasGroup.alpha = alpha;

                await UniTask.Yield(_alphaCTS.Token);
            }

            if (startTransition)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }

            IsInPressedTransition = false;
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            Press();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Press();
        }
    }
}
