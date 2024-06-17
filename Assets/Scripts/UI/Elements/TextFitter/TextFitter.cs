using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [ExecuteInEditMode]
    public class TextFitter : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        [Title("Setings")]
        [SerializeField] private float _horizontalPadding;
        [SerializeField] private float _verticalPadding;
        [SerializeField, HideIf("usedByController")] private Vector2 _minSize;

        [Title("Read Only")]
        [ReadOnly] public float PreferredTextWidth;
        [ReadOnly] public float PreferredTextHeight;

        public Image Image => _image;
        public Vector2 PreferredImageSize { get; private set; }

        public event Action<Vector2> OnPreferredSizeChanged;
        [HideInInspector] public bool usedByController = false;

        private Vector2 _cashedImageSize;
        private float _cashedTextWidth;
        private float _cashedTextHeight;

        [Button, HideIf("usedByController")]
        public void SetCurrentSizeAsMin() => _minSize = _image.rectTransform.sizeDelta;

        [ContextMenu("Reset Used By Controller")]
        public void ResetUsedByController() => usedByController = false;


        [Button]
        public void ForceAdjust()
        {
            AdjustImage();
            _image.rectTransform.ForceUpdateRectTransforms();
            _cashedImageSize = _image.rectTransform.sizeDelta;
            _cashedTextWidth = PreferredTextWidth;
            _cashedTextHeight = PreferredTextHeight;
        }

        private void Start()
        {
            ForceAdjust();
        }

        private void Update()
        {
            if (_image != null && _text != null)
            {
                PreferredTextWidth = _text.preferredWidth;
                PreferredTextHeight = _text.preferredHeight;
                if (_image.rectTransform.sizeDelta != _cashedImageSize ||
                    PreferredTextWidth != _cashedTextWidth ||
                    PreferredTextHeight != _cashedTextHeight)
                {
                    ForceAdjust();
                }
            }
        }

        public void AdjustImage()
        {
            PreferredImageSize = GetTextSizeWithPadding();
            if (!usedByController)
            {
                _image.rectTransform.sizeDelta = PreferredImageSize;
            }
            OnPreferredSizeChanged?.Invoke(PreferredImageSize);
        }

        public Vector2 GetTextSizeWithPadding()
        {
            var preferredImageSize = new Vector2(PreferredTextWidth + _horizontalPadding * 2, PreferredTextHeight + _verticalPadding * 2);
            return new Vector2(Mathf.Max(_minSize.x, preferredImageSize.x), Mathf.Max(_minSize.y, preferredImageSize.y));
        }
    }
}
