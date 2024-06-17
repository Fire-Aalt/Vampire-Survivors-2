using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CoreSystem
{
    public class CoreComponent : MonoBehaviour
    {
        protected Core core;
        protected virtual void Awake()
        {
            core = GetComponentInParent<Core>();

            if (core == null)
            {
                Debug.LogError("No Core on the parent");
            }
            core.AddComponent(this);
        }

        protected virtual void Start()
        {
        }

        public virtual void LogicUpdate()
        {
        }
    }
}
