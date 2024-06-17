using Game.CoreSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Entity : MonoBehaviour
    {
        public const string MOVE_ANIM_SPEED_NAME = "MoveSpeed";
        [field: SerializeField, Title("Animations")] public Animator Animator { get; protected set; }
        [SerializeField] protected float _maxAffectedVelocity;
        [SerializeField] protected AnimationCurve _moveAnimSpeedCurve;

        public Core Core { get; protected set; }
        public Collider2D BodyCollider { get; protected set; }

        public Movement Movement { get; protected set; }
        protected bool initialized;

        protected int moveAnimSpeedNameID = Animator.StringToHash(MOVE_ANIM_SPEED_NAME);

        protected virtual void Awake()
        {
            BodyCollider = GetComponent<Collider2D>();
            Core = GetComponentInChildren<Core>(true);

            Movement = Core.GetCoreComponent<Movement>();
        }

        protected virtual void Update()
        {
            float normSpeed = Mathf.InverseLerp(0f, _maxAffectedVelocity, Movement.Magnitude);
            Animator.SetFloat(moveAnimSpeedNameID, _moveAnimSpeedCurve.Evaluate(normSpeed));
        }
    }
}
