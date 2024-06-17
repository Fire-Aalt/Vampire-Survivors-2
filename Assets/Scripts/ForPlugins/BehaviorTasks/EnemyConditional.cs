using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemyConditional : Conditional
    {
        protected Enemy Enemy { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();

            TryGetComponent(out Enemy enemy);
            Enemy = enemy;

            if (Enemy == null)
            {
                Debug.LogError(Owner + " uses ConditionalTask that requires Enemy script attached");
            }
        }
    }
}
