using UnityEngine;
using InsideVentura.AI;

// Теперь нам НЕ НУЖЕН Animator
[RequireComponent(typeof(EnemyBrain))]
public class Envy : Enemy
{
    [Header("Bite Settings")]
    public float biteDistance = 1.2f;
    public float biteCooldown = 1.0f;
    [SerializeField] private AttackObject biteAttackPrefab;

    private SimpleEnemyAnimator _view;
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
        Brain.Init(this, new Envy_ChaseState());
    }

    private void Update()
    {
        if (IsDead) return;
        if (_curCooldown > 0) _curCooldown -= Time.deltaTime;
    }

    // Запускаем последовательность укуса через наш новый аниматор
    public void StartBiteSequence()
    {
        _view.PlayAttack(EnvyDoBite, EnvyStartCooldown);
    }

    private void EnvyDoBite()
    {
        if (target == null) return;

        // Определяем направление укуса
        Vector2 diff = (target.position - transform.position).normalized;
        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, rotZ - 90f);

        // Спавним хитбокс укуса
        var attackObj = GamePools.Hitboxes.Get(biteAttackPrefab, transform.position, rot);
        attackObj.gameObject.SetActive(true);
        attackObj.Initialize(damage, LayerMask.GetMask("Player"));
    }

    private void EnvyStartCooldown()
    {
        _curCooldown = biteCooldown;
        Brain.ChangeState(new Envy_ChaseState());
    }

    protected override void Die()
    {
        base.Die();
        Brain.enabled = false;
        enabled = false;
    }
}