using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    [ExecuteAlways]
    public class TextFitterController : MonoBehaviour
    {
        [SerializeField, OnValueChanged("HandleArrayChanged")] private TextFitter[] _textFitters = new TextFitter[0];
        [SerializeField] private Vector2 _minSize;

        private TextFitter[] _cashedElements = new TextFitter[0];
        private Vector2 _maxCashedSize;

        [Button]
        public void FindTextFitters() => _textFitters = GetComponentsInChildren<TextFitter>();

        [Button]
        public void SetCurrentSizeAsMin() => _minSize = _textFitters[0].Image.rectTransform.sizeDelta;

        private void HandleArrayChanged()
        {
            var newMaxSize = _maxCashedSize;
            // Added
            var addedElements = _textFitters.Except(_cashedElements);
            foreach (var element in addedElements)
            {
                element.usedByController = true;
                element.OnPreferredSizeChanged += RecalculateUI;

                if (element.PreferredImageSize.x > newMaxSize.x)
                {
                    newMaxSize = element.PreferredImageSize;
                }
            }
            // Removed
            var removedElements = _cashedElements.Except(_textFitters);
            foreach (var element in removedElements)
            {
                element.usedByController = false;
                element.OnPreferredSizeChanged -= RecalculateUI;
            }
            // Recalculate
            RecalculateUI(newMaxSize);
            _cashedElements = _textFitters;
        }

        private void RecalculateUI(Vector2 newImageSize)
        {
            if (newImageSize.x >= _maxCashedSize.x)
            {
                _maxCashedSize = newImageSize;
            }
            else
            {
                var newMaxSize = Vector2.zero;
                foreach (var textFitter in _textFitters)
                {
                    if (textFitter.PreferredImageSize.x > newMaxSize.x)
                    {
                        newMaxSize = textFitter.PreferredImageSize;
                    }
                }
                _maxCashedSize = newMaxSize;
            }

            ApplyNewSize(_maxCashedSize);
        }

        private void ApplyNewSize(Vector2 size)
        {
            if (size.x < _minSize.x)
            {
                size = _minSize;
            }

            foreach (var textFitter in _textFitters)
            {
                textFitter.Image.rectTransform.sizeDelta = size;
            }
        }

        private void OnEnable()
        {
            foreach (var textFitter in _textFitters)
            {
                textFitter.usedByController = true;
                textFitter.OnPreferredSizeChanged -= RecalculateUI;
                textFitter.OnPreferredSizeChanged += RecalculateUI;
            }
            _cashedElements = _textFitters;
        }

        private void OnDestroy()
        {
            foreach (var textFitter in _textFitters)
            {
                textFitter.usedByController = false;
            }
        }
    }
}
