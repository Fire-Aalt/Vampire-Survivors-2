using Lean.Pool;
using UnityEngine;

namespace Game
{
    public class PoolableParticle : MonoBehaviour, IPoolable
    {
        public void OnDespawn()
        {
            
        }

        public void OnSpawn()
        {
            
        }

        private void OnParticleSystemStopped()
        {
            LeanPool.Despawn(gameObject);
        }

    }
}
