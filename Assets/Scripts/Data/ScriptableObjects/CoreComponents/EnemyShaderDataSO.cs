using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "ShaderData/Enemy Shader Data")]
    public class EnemyShaderDataSO : ScriptableObject
    {
        [Title("Flash Settings")]
        public float flashDuration;
        public bool useFlashAnimationCurve;
        [ShowIf("useFlashAnimationCurve")] public AnimationCurve flashCurve;

        [Title("Dissolve Settings")]
        public float dissolveDuration;
        public bool useDissolveAnimationCurve;
        [ShowIf("useDissolveAnimationCurve")] public AnimationCurve dissolveCurve;
    }
}
