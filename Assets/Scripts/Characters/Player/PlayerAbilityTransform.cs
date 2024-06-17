using Game.CoreSystem;
using UnityEngine;

namespace Game
{
    public class PlayerAbilityTransform : MonoBehaviour
    {
        [SerializeField] private NormalizeMode _mode;
        private PlayerMovement _movement;

        private void Start()
        {
            _movement = GetComponentInParent<Player>().Core.GetCoreComponent<PlayerMovement>();
            _movement.OnFlipped += NormalizeTransform;
        }

        private void LateUpdate()
        {
            NormalizeTransform();
        }

        private void NormalizeTransform()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            switch (_mode)
            {
                case NormalizeMode.NoFlip:
                    transform.rotation = Quaternion.Euler(0f, 0f, euler.z);
                    break;
                case NormalizeMode.NoLean:
                    transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
                    break;
                case NormalizeMode.Static:
                    transform.rotation = Quaternion.identity;
                    break;
                default:
                    return;
            }
        }

        enum NormalizeMode
        {
            None,
            NoFlip,
            NoLean,
            Static
        }
    }
}
