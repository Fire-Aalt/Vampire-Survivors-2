using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    [RequireComponent(typeof(SelectableRect))]
    public class ElementButton : UnityEngine.UI.Button
    {
        public TextButtonSO data;

        protected CancellationTokenSource _scaleCTS;
        protected Vector3 _startScale;

        public SelectableRect SelectableRect { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_startScale == Vector3.zero)
                _startScale = transform.localScale;

            transform.localScale = _startScale;
        }

        protected override void Awake()
        {
            base.Awake();

            SelectableRect = GetComponent<SelectableRect>();
        }

        private async UniTask ScaleButtonDown(bool startTransition)
        {
            _scaleCTS = new CancellationTokenSource();
            Vector3 startScale = transform.localScale;
            Vector3 endScale;

            if (startTransition)
            {
                endScale = _startScale * data.scaleAmount;
                SelectableRect.Deselect();
            }
            else
            {
                endScale = _startScale;
            }
            float duration = data.duration;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                Vector3 lerpedScale = Vector3.Lerp(startScale, endScale, (elapsedTime / duration));
                transform.localScale = lerpedScale;

                await UniTask.Yield(_scaleCTS.Token);
            }
        }

        public virtual void Press()
        {
            data.clickSfx.Play(transform.position);
            _scaleCTS?.Cancel();
            ResetButton().Forget();
        }

        public async virtual UniTask ResetButton()
        {
            await ScaleButtonDown(true);
            ScaleButtonDown(false).Forget();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            eventData.selectedObject = gameObject;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            eventData.selectedObject = null;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            SelectableRect.Select();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            SelectableRect.Deselect();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Press();
            eventData.selectedObject = null;
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            Press();
            eventData.selectedObject = null;
        }
    }
}
