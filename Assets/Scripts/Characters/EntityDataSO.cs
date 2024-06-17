using Sirenix.OdinInspector;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
    public abstract class EntityDataSO : ScriptableObject
    {
        [ReadOnly] public string entityGuid;

#if UNITY_EDITOR
        [ContextMenu("Reassign Guid (WARNING: Save Data will be invalid and reset)")]
        private void ReassignGuid()
        {
            entityGuid = GUID.Generate().ToString();
        }
#endif
    }
}
