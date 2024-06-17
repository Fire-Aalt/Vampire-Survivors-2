using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "UI/SelectableUIShader", fileName = "SelectableUIShader")]
    public class SelectableUIShaderControllerSO : ScriptableObject
    {
        public Color mainTexColor;
        public float duration;

        [Title("Start Values")]
        public Color startOutlineColor;
        [Range(1f, 100f)] public float startOutlineGlow;
        [Range(0f, 0.2f)] public float startOutlineWidth;
        [Range(0f, 2f)] public float startOutlineDistortionAmount;

        [Title("Target Values")]
        public Color targetOutlineColor;
        [Range(1f, 100f)] public float targetOutlineGlow;
        [Range(0f, 0.2f)] public float targetOutlineWidth;
        [Range(0f, 2f)] public float targetOutlineDistortionAmount;
    }
}
