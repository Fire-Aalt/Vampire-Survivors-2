using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "UI/Roundabout Settings")]
    public class OptionSelectionSO : ScriptableObject
    {
        [Title("Settings")]
        public float horizontalPadding;
        public float verticalPadding;
        public float spacing;
        public bool roundabout;
        public Color arrowColor;
        [HideIf("roundabout")] public Color disabledArrowColor;
        public SoundEffectSO clickSfx;
    }
}
