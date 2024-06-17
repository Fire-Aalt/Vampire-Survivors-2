using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SelectableRect : MonoBehaviour
    {
        public SelectableUIShaderControllerSO data;
        [SerializeField] private SoundEffectSO _selectSfx;

        private Image _image;
        private SelectableUIShaderController _shaderController;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = data.mainTexColor;

            var instancedMat = new Material(_image.material);
            _image.material = instancedMat;
            _shaderController = new SelectableUIShaderController(instancedMat, data, gameObject.GetCancellationTokenOnDestroy(), true);
        }

        [Button]
        public void SelectPreview()
        {
            GetReferencesEditor();
            Select();
        }

        [Button]
        public void DeselectPreview()
        {
            GetReferencesEditor();
            Deselect();
        }

        public void Show(float duration)
        {
            _shaderController.ShowOutline(duration);
        }

        public void Hide(float duration)
        {
            _shaderController.HideOutline(duration);
        }

        public void Select()
        {
            _shaderController.BeginSelect();
            _selectSfx?.Play(transform.position);
        }

        public void Deselect()
        {
            _shaderController.EndSelect();
        }

        private void GetReferencesEditor()
        {
            if (_image == null)
                _image = GetComponent<Image>();
            _image.color = data.mainTexColor;
            _shaderController = new SelectableUIShaderController(_image.material, data, gameObject.GetCancellationTokenOnDestroy(), false);
        }
    }
}
