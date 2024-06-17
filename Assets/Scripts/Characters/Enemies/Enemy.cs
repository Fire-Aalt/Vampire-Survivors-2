using BehaviorDesigner.Runtime;
using Game;
using Game.CoreSystem;
using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[RequireComponent(typeof(OutOfBoundsDespawner))]
public class Enemy : Entity, IPoolable
{
    public static event Action<Enemy> OnVisible, OnHidden, OnDeath;

    [field: SerializeField, InlineEditor, Title("Data")] public EnemyDataSO Data { get; set; }
    [field: SerializeField] public Hitbox BodyHitbox { get; private set; }
    [field: SerializeField] public OutOfBoundsDespawner Despawner { get; private set; }

    public Transform Target { get; private set; }
    public EnemyStats Stats { get; private set; }
    public BehaviorTree BehaviorTree { get; private set; }

    private SpriteShaderController _shaderController;

    protected override void Awake()
    {
        base.Awake();
        BehaviorTree = GetComponent<BehaviorTree>();

        _shaderController = Core.GetCoreComponent<SpriteShaderController>();
        Stats = Core.GetCoreComponent<EnemyStats>();
    }

    private void Start()
    {
        Target = Player.Active.transform;
        Stats.MaxHealth = Data.stats.health;
        Stats.Defense = Data.stats.defence;
        BodyHitbox.damageToPlayer = Data.stats.damage;
    }

    private void Death()
    {
        BodyCollider.enabled = false;
        OnDeath?.Invoke(this);
        BehaviorTree.DisableBehavior();
        LeanPool.Despawn(gameObject, 1f);
    }

    public void OnSpawn()
    {
        Stats.Revive(1f);

        BodyCollider.enabled = true;
        _shaderController.ResetEffects();
        Movement.ResetFlip();

        BehaviorTree.EnableBehavior();
    }

    public void OnDespawn()
    {
        
    }

    private void HandleVisible() => OnVisible?.Invoke(this);
    private void HandleHidden() => OnHidden?.Invoke(this);

    private void OnEnable()
    {
        Stats.OnDeath += Death;
        Despawner.OnVisible += HandleVisible;
        Despawner.OnHidden += HandleHidden;
    }

    private void OnDisable()
    {
        Stats.OnDeath -= Death;
        Despawner.OnVisible -= HandleVisible;
        Despawner.OnHidden -= HandleHidden;
    }
}
