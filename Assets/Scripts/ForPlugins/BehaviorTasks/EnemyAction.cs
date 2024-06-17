using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Game
{
    public abstract class EnemyAction : Action
    {
        protected Enemy Enemy { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();

            TryGetComponent(out Enemy enemy);
            Enemy = enemy;

            if (Enemy == null)
            {
                Debug.LogError(Owner + " uses ActionTask that requires Enemy script attached");
            }
        }
    }
}
