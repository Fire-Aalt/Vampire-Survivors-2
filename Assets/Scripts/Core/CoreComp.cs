using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CoreSystem
{
    public class CoreComp<T> where T : CoreComponent
    {
        private Core core;
        private T comp;

        public T Comp => comp ??= core.GetCoreComponent<T>();

        public CoreComp(Core core)
        {
            if (core == null)
                Debug.LogWarning($"Core is Null for component {typeof(T)}");

            this.core = core;
        }
    }
}