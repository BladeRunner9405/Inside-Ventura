using UnityEngine;
using InsideVentura.AI;

[RequireComponent(typeof(EnemyBrain))]
public class UncontrollableAnger : Enemy
{
    // Поле visualData УДАЛЕНО - оно больше здесь не нужно!
    private SimpleEnemyAnimator _view;

    [Header("Distances")]
    public float playerDistance = 2.7f;
    public float retreatDistance = 2f;

    [Header("Attack Settings")]
    public float cooldownDuration = 3f;
    [SerializeField] private AttackObject waveAttackPrefab;

    private float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;

    protected override void Awake()
    {
        base.Awake();
        _view = GetComponentInChildren<SimpleEnemyAnimator>();
    }

    protected override void Start()
    {
        base.Start();
        Brain.Init(this, new UA_ChaseState());
    }

    private void Update()
    {
        if (IsDead) return;
        if (_curCooldown > 0) _curCooldown -= Time.deltaTime;
    }

    // Метод начала атаки, который вызовет наше состояние
    public void StartAttackSequence()
    {
        // Просто просим аниматор сыграть атаку
        _view.PlayAttack(UADoDamage, UAStartCooldown);
    }

    private void UADoDamage()
    {
        if (target == null) return;
        Vector2 diff = (target.position - transform.position).normalized;
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, rotZ - 90f);

        var attackObj = GamePools.Hitboxes.Get(waveAttackPrefab, transform.position, rot);
        attackObj.gameObject.SetActive(true);
        attackObj.Initialize(damage, LayerMask.GetMask("Player"), 0f);
    }

    private void UAStartCooldown()
    {
        _curCooldown = cooldownDuration;
        Brain.ChangeState(new UA_ChaseState());
    }

    protected override void Die()
    {
        base.Die();
        Brain.enabled = false;
        enabled = false;
    }
}