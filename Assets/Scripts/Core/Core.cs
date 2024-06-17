using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.CoreSystem
{
    public class Core : MonoBehaviour
    {
        private readonly List<CoreComponent> CoreComponents = new();


        protected virtual void Awake()
        {
        
        }

        public virtual void LogicUpdate()
        {
            foreach (CoreComponent component in CoreComponents) 
            {
                component.LogicUpdate();
            }
        }

        public void AddComponent(CoreComponent component)
        {
            if (!CoreComponents.Contains(component))
            {
                CoreComponents.Add(component);
            }
        }

        public bool HasCoreComponent<T>() where T : CoreComponent
        {
            var comp = CoreComponents.OfType<T>().FirstOrDefault();

            if (comp)
                return true;

            comp = GetComponentInChildren<T>();

            if (comp)
                return true;

            return false;
        }

        public T GetCoreComponent<T>() where T : CoreComponent
        {
            var comp = CoreComponents.OfType<T>().FirstOrDefault();

            if (comp)
                return comp;

            comp = GetComponentInChildren<T>();

            if (comp)
                return comp;

            Debug.LogWarning($"{typeof(T)} not found on {transform.parent.name}");

            return null;
        }
    }
}

