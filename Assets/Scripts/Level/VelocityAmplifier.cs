using Game.CoreSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class VelocityAmplifier : MonoBehaviour
    {
        [SerializeField] private float multiplier = 1f;

        private readonly Dictionary<Collider2D, Entity> _affectedDict = new();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_affectedDict.ContainsKey(collision) && collision.TryGetComponent<Entity>(out var entity))
            {
                entity.Movement.VelocityMultiplier = multiplier;
                _affectedDict.Add(collision, entity);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_affectedDict.ContainsKey(collision))
            {
                _affectedDict[collision].Movement.VelocityMultiplier = 1f;
                _affectedDict.Remove(collision);
            }
        }
    }
}
