using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class DevouringEnvy : Enemy
{
    [Header("Envy Distances")]
    [Tooltip("Дистанция, на которой Зависть начинает кусать")]
    public float attackDistance = 1.1f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;

    [SerializeField]
    private AttackObject biteAttackPrefab;

    private float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;

    private Vector2 _attackDir;

    protected override void Start()
    {
        base.Start();
        // Зависть всегда начинает с преследования
        Brain.Init(this, new Envy_ChaseState());
    }

    protected override void Update()
    {
        base.Update();
        if (IsDead) return;

        if (_curCooldown > 0)
            _curCooldown -= Time.deltaTime;
    }

    public override void Attack(Vector2 direction)
    {
        _attackDir = direction;
        // Зависть кусает часто, анимация должна быть быстрой
        _animator.SetTrigger(animAttack);
    }

    public override void OnAnimationEvent_Impact()
    {
        if (target == null || IsDead) return;

        var attackObj = GamePools.Hitboxes.Get(biteAttackPrefab, transform.position, Quaternion.identity);
        attackObj.gameObject.SetActive(true);
        attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir);
    }

    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        _curCooldown = attackCooldown;
        Brain.ChangeState(new Envy_ChaseState());
    }
}