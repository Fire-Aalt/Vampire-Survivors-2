using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "UI/TextButton", fileName = "TextButton")]
    public class TextButtonSO : ScriptableObject
    {
        public float duration = 0.2f;
        public float scaleAmount = 0.7f;
        public SoundEffectSO clickSfx;
    }
}
