using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class SelectableUIShaderController
    {
        private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineGlow = Shader.PropertyToID("_OutlineGlow");
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int OutlineDistortionAmount = Shader.PropertyToID("_OutlineDistortAmount");

        private CancellationTokenSource _transitionCTS;
        private CancellationTokenSource _alphaCTS;

        private readonly Material _material;
        private readonly SelectableUIShaderControllerSO _data;
        private readonly CancellationToken _destroyToken;

        private float _transitionT;
        private float _alphaT;

        public SelectableUIShaderController(Material material, SelectableUIShaderControllerSO data, CancellationToken destroyToken, bool reset)
        {
            _material = material;
            _data = data;
            _destroyToken = destroyToken;

            if (reset)
            {
                Transition(0f, startTransition: false).Forget();
            }
        }

        public void ShowOutline(float duration)
        {
            LerpAlpha(duration, showTransition: true).Forget();
        }

        public void HideOutline(float duration)
        {
            LerpAlpha(duration, showTransition: false).Forget();
        }

        public void BeginSelect()
        {
            Transition(_data.duration, startTransition: true).Forget();
        }

        public void EndSelect()
        {
            Transition(_data.duration, startTransition: false).Forget();
        }

        private async UniTask LerpAlpha(float duration, bool showTransition)
        {
            _alphaCTS?.Cancel();
            _alphaCTS = new CancellationTokenSource();

            float startOutlineAlpha = _material.GetFloat(OutlineAlpha);
            float targetOultineAlpha;

            if (showTransition)
            {
                targetOultineAlpha = 1f;

                duration *= 1 - _alphaT;
            }
            else
            {
                targetOultineAlpha = 0f;

                duration *= _alphaT;
            }

            float elapsedTime = 0f;
            float transitionBuffer = _alphaT;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                float t = elapsedTime / duration;
                if (showTransition)
                {
                    _alphaT = transitionBuffer + t * (1 - transitionBuffer);
                }
                else
                {
                    _alphaT = transitionBuffer - t * transitionBuffer;
                }

                // Transition
                float lerpedOutlineAlpha = Mathf.Lerp(startOutlineAlpha, targetOultineAlpha, t);

                _material.SetFloat(OutlineAlpha, lerpedOutlineAlpha);

                if (!_destroyToken.IsCancellationRequested)
                {
                    await UniTask.Yield(_alphaCTS.Token);
                }
            }

            _material.SetFloat(OutlineAlpha, targetOultineAlpha);
        }

        private async UniTask Transition(float duration, bool startTransition)
        {
            _transitionCTS?.Cancel();
            _transitionCTS = new CancellationTokenSource();

            // Setup
            Color startOutlineColor = _material.GetColor(OutlineColor);
            float startOutlineGlow = _material.GetFloat(OutlineGlow);
            float startOutlineWidth = _material.GetFloat(OutlineWidth);
            float startOutlineDistortionAmount = _material.GetFloat(OutlineDistortionAmount);

            Color targetOutlineColor;
            float targetOutlineGlow;
            float targetOutlineWidth;
            float targetOutlineDistortionAmount;

            if (startTransition)
            {
                targetOutlineColor = _data.targetOutlineColor;
                targetOutlineGlow = _data.targetOutlineGlow;
                targetOutlineWidth = _data.targetOutlineWidth;
                targetOutlineDistortionAmount = _data.targetOutlineDistortionAmount;

                duration *= 1 - _transitionT;
            }
            else
            {
                targetOutlineColor = _data.startOutlineColor;
                targetOutlineGlow = _data.startOutlineGlow;
                targetOutlineWidth = _data.startOutlineWidth;
                targetOutlineDistortionAmount = _data.startOutlineDistortionAmount;

                duration *= _transitionT;
            }

            float elapsedTime = 0f;
            float transitionBuffer = _transitionT;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                float t = elapsedTime / duration;
                if (startTransition)
                {
                    _transitionT = transitionBuffer + t * (1 - transitionBuffer);
                }
                else
                {
                    _transitionT = transitionBuffer - t * transitionBuffer;
                }

                // Transition
                Color lerpedOutlineColor = Color.Lerp(startOutlineColor, targetOutlineColor, t);
                float lerpedOutlineGlow = Mathf.Lerp(startOutlineGlow, targetOutlineGlow, t);
                float lerpedOutlineWidth = Mathf.Lerp(startOutlineWidth, targetOutlineWidth, t);
                float lerpedOutlineDistortionAmount = Mathf.Lerp(startOutlineDistortionAmount, targetOutlineDistortionAmount, t);

                _material.SetColor(OutlineColor, lerpedOutlineColor);
                _material.SetFloat(OutlineGlow, lerpedOutlineGlow);
                _material.SetFloat(OutlineWidth, lerpedOutlineWidth);
                _material.SetFloat(OutlineDistortionAmount, lerpedOutlineDistortionAmount);

                if (!_destroyToken.IsCancellationRequested)
                {
                    await UniTask.Yield(_transitionCTS.Token);
                }
            }

            _material.SetColor(OutlineColor, targetOutlineColor);
            _material.SetFloat(OutlineGlow, targetOutlineGlow);
            _material.SetFloat(OutlineWidth, targetOutlineWidth);
            _material.SetFloat(OutlineDistortionAmount, targetOutlineDistortionAmount);

            // Tear down
            if (!startTransition)
            {
                _transitionCTS?.Cancel();
            }
        }
    }
}
