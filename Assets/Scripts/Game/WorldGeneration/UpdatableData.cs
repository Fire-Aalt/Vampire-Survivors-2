using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class UpdatableData : ScriptableObject
    {
        public event System.Action OnValuesUpdated;
        public bool autoUpdate;

        protected virtual void OnValidate()
        {
            if (autoUpdate)
            {
                NotifyOfUpdatedValues();
            }
        }

        [Button]
        public void NotifyOfUpdatedValues()
        {
            OnValuesUpdated?.Invoke();
        }
    }
}
