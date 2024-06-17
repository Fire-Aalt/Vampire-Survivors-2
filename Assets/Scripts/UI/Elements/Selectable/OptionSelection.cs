using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(SelectableRect))]
    public class OptionSelection : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Header("References")]
        [SerializeField] private Image _leftArrow;
        [SerializeField] private TextMeshProUGUI _optionLabel;
        [SerializeField] private Image _rightArrow;
        [SerializeField] private ApplyButton _applyButton;

        [Header("Data")]
        [SerializeField] private OptionSelectionSO _data;
        [SerializeField] private bool _useApplyButton = false;

        public UnityEvent<int> OnValueChanged;

        public string[] Options { get; private set; }
        public OptionSelectionSO Data { get => _data; }
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }
        private RectTransform _rectTransform;

        public int RawSelectedIndex { get => _rawSelectedIndex; set {
                if (_data.roundabout)
                {
                    if (value < 0)
                    {
                        _rawSelectedIndex = Options.Length - 1;
                    }
                    else if (value > Options.Length - 1)
                    {
                        _rawSelectedIndex = 0;
                    }
                    else
                    {
                        _rawSelectedIndex = value;
                    }
                }
                else if (value >= 0 && value < Options.Length)
                {
                    _rawSelectedIndex = value;
                }

                if (!_useApplyButton)
                {
                    SelectedIndex = _rawSelectedIndex;
                    OnValueChanged?.Invoke(SelectedIndex);
                }
                else if (SelectedIndex != _rawSelectedIndex)
                {
                    _applyButton.Appear();
                }
                else if (SelectedIndex == _rawSelectedIndex)
                {
                    _applyButton.Disappear();
                }
            } }
        private int _rawSelectedIndex;

        public int SelectedIndex { get => _selectedIndex; private set => _selectedIndex = value; }
        private int _selectedIndex;

        public void HandleValueApplied()
        {
            SelectedIndex = RawSelectedIndex;
            OnValueChanged?.Invoke(SelectedIndex);
        }

        private SelectableRect _selectableRect;
        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                base.Awake();
                _selectableRect = GetComponent<SelectableRect>();
            }
        }

        private Camera _uiCamera;
        protected override void Start()
        {
            if (Application.isPlaying)
            {
                _uiCamera = CameraManager.Active.UICamera;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying && !NullReferenceExist())
            {
                AutoResize();
            }
        }

        public void InitializeNewOptions(string[] options, int defaultIndex)
        {
            Options = options;

            string longestOption = string.Empty;
            foreach (string option in options)
            {
                if (option.Length >= longestOption.Length)
                {
                    longestOption = option;
                }
            }

            _optionLabel.text = longestOption;
            _rawSelectedIndex = defaultIndex;
            _selectedIndex = defaultIndex;
            DelayedUpdateRectSize(true).Forget();
        }

        public void RefreshUI()
        {
            _data.clickSfx.Play(transform.position);
            _optionLabel.text = Options[RawSelectedIndex];
            if (RawSelectedIndex == 0 && !_data.roundabout)
            {
                _leftArrow.color = _data.disabledArrowColor;
                _rightArrow.color = _data.arrowColor;
            }
            else if (RawSelectedIndex == Options.Length - 1 && !_data.roundabout)
            {
                _leftArrow.color = _data.arrowColor;
                _rightArrow.color = _data.disabledArrowColor;
            }
            else
            {
                _leftArrow.color = _data.arrowColor;
                _rightArrow.color = _data.arrowColor;
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            if (!_applyButton.IsInPressedTransition && eventData.moveVector == Vector2.left || eventData.moveVector == Vector2.right)
            {
                RawSelectedIndex += (int)eventData.moveVector.x;
                RefreshUI();
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _selectableRect.Select();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _selectableRect.Deselect();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (_useApplyButton && eventData.selectedObject == _applyButton)
            {
                _selectableRect.Select();
            }
            else
            {
                eventData.selectedObject = gameObject;
                if (_useApplyButton && SelectedIndex != RawSelectedIndex)
                {
                    _applyButton.SelectableRect.Select();
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (_useApplyButton && eventData.selectedObject == _applyButton)
            {
                _selectableRect.Deselect();
            }
            else
            {
                eventData.selectedObject = null;
                if (_useApplyButton && SelectedIndex != RawSelectedIndex)
                {
                    _applyButton.SelectableRect.Deselect();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.pressPosition, _uiCamera, out Vector2 localMousePos);

            if (!_applyButton.IsInPressedTransition && rectTransform.rect.Contains(localMousePos))
            {
                if (localMousePos.x < rectTransform.rect.x + rectTransform.rect.size.x / 2)
                {
                    RawSelectedIndex -= 1;
                }
                else if (localMousePos.x >= rectTransform.rect.x + rectTransform.rect.size.x / 2)
                {
                    RawSelectedIndex += 1;
                }
                RefreshUI();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (_useApplyButton && !_applyButton.IsInPressedTransition && SelectedIndex != RawSelectedIndex)
            {
                _applyButton.Press();
                HandleValueApplied();
            }
        }

        private async UniTaskVoid DelayedUpdateRectSize(bool refreshUI)
        {
            await UniTask.WaitForEndOfFrame(this);
            UpdateRectSize(refreshUI);
        }

        private void UpdateRectSize(bool refreshUI)
        {
            var textSize = _optionLabel.GetPreferredValues();
            rectTransform.sizeDelta = new Vector2(textSize.x + Data.horizontalPadding * 2, textSize.y + Data.verticalPadding * 2);
            if (refreshUI)
            {
                RefreshUI();
            }
        }

        private bool NullReferenceExist()
        {
            return _leftArrow == null || _rightArrow == null ||
                _optionLabel == null || _data == null;
        }

        private float _cachedSpacing;
        private Vector2 _cachedPreferredTextSize;
        private void AutoResize()
        {
            
            var preferredSize = _optionLabel.GetPreferredValues();
            if (preferredSize != _cachedPreferredTextSize || _data.spacing != _cachedSpacing)
            {
                _cachedPreferredTextSize = preferredSize;
                _cachedSpacing = _data.spacing;
                UpdateRectSize(false);
            }
        }
    }
}
