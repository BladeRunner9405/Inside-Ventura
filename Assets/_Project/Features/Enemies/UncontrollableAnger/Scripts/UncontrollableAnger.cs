using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class UncontrollableAnger : Enemy
{
    [Header("Distances")]
    public float playerDistance = 2.7f;
    public float retreatDistance = 2f;

    [Header("Attack Settings")]
    public float cooldownDuration = 3f;

    [SerializeField]
    private AttackObject waveAttackPrefab;

    private float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;

    private Vector2 _attackDir;

    protected override void Start()
    {
        base.Start();
        Brain.Init(this, new UA_ChaseState());
    }

    private void Update()
    {
        base.Update();
        if (IsDead) return;
        
        if (_curCooldown > 0)
            _curCooldown -= Time.deltaTime;
    }

    public override void Attack(Vector2 direction)
    {
        _attackDir = (target.position - transform.position).normalized;
        
        // Запускаем триггер атаки во встроенном аниматоре
        if (_animator != null) _animator.SetTrigger(animAttack);
    }

    // --- ANIMATION EVENTS ---

    // 1. Вызывается Unity Animator'ом на кадре удара (спавн волны)
    public override void OnAnimationEvent_Impact()
    {
        if (target == null || IsDead) return;

        var attackObj = GamePools.Hitboxes.Get(
            waveAttackPrefab,
            transform.position,
            Quaternion.identity
        );
        attackObj.gameObject.SetActive(true);

        attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir);
    }

    // 2. Вызывается Unity Animator'ом в конце анимации
    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        _curCooldown = cooldownDuration;
        Brain.ChangeState(new UA_ChaseState());
    }
}