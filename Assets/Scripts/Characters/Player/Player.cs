using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;
using Game;
using Game.CoreSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnDeath;
    public static Vector2 AimDirection { get; private set; }

    public static Player Active {
            get {
                if (_current == null)
                    _current = FindObjectOfType<Player>();
                return _current;
            }
        }
    private static Player _current;
    public static bool IsAlive { get; private set; }

    [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
    [field: SerializeField] public AbilityController AbilityController { get; private set; }

    [Title("Stats")]
    public float backwardSpeedMultiplier;

    private PlayerMovement _playerMovement;
    private PlayerStats _stats;
    private Camera _mainCamera;

    protected override void Awake()
    {
        base.Awake();

        _playerMovement = Core.GetCoreComponent<PlayerMovement>();
        _stats = Core.GetCoreComponent<PlayerStats>();
        _mainCamera = CameraManager.Active.MainCamera;
    }

    private void Start()
    {
        Animator.SetBool("Idle", true);
        IsAlive = true;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.timeScale == 0f) return;

        if (EnemyManager.ClosestEnemyToPlayer == null || !AbilityController.AutiomaticWeapons)
        {
            AimDirection = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
        else
        {
            AimDirection = EnemyManager.ClosestEnemyToPlayer.transform.position - transform.position;
        }

        if (!_stats.IsAlive)
        {
            return;
        }

        Core.LogicUpdate();
        AbilityController.LogicUpdate();
        CheckAnimations();
    }

    private void FixedUpdate()
    {
        if (!_stats.IsAlive)
        {
            return;
        }

        if (Math.Sign(InputHandler.Movement.x) == -_playerMovement.FacingDirection)
            _playerMovement.Move(InputHandler.Movement, 1f, backwardSpeedMultiplier);
        else
            _playerMovement.Move(InputHandler.Movement, 1f, 1f);

        _playerMovement.CheckIfShouldFlip(AimDirection.x);
    }

    private void CheckAnimations()
    {
        if (Animator.GetBool("Idle") && InputHandler.Movement != Vector2.zero)
        {
            Animator.SetBool("Idle", false);
            Animator.SetBool("Walk", true);
        }
        else if (Animator.GetBool("Walk") && InputHandler.Movement == Vector2.zero)
        {
            Animator.SetBool("Idle", true);
            Animator.SetBool("Walk", false);
        }
    }

    public void Death()
    {
        _playerMovement.SetVelocityZero();
        IsAlive = false;
        Animator.Play("Death");
        OnDeath?.Invoke();
    }

    private void OnEnable()
    {
        _stats.OnDeath += Death;
    }

    private void OnDisable()
    {
        _stats.OnDeath -= Death;
    }
}
