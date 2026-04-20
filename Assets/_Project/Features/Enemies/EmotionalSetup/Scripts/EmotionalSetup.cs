using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class EmotionalSetup : Enemy
{
    [Header("Setup Attack Settings")]
    public float attackCooldown = 2f;
    public float orbSpeed = 0.7f;
    
    [SerializeField] 
    private AttackObject darkOrbPrefab;

    private float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;

    protected override void Start()
    {
        base.Start();
        // Враг не двигается, поэтому инициализируем его сразу в состоянии покоя
        Brain.Init(this, new ES_IdleState());
    }
    
    private void Update()
    {
        if (IsDead) return;

        // Таймер перезарядки
        if (_curCooldown > 0)
            _curCooldown -= Time.deltaTime;
    }

    public override void Attack(Vector2 direction)
    {
        // Запускаем анимацию стрельбы
        if (_animator != null) _animator.SetTrigger(animAttack);
    }

    // --- ANIMATION EVENTS ---

    // 1. Вызывается Unity Animator'ом на кадре выстрела
    public override void OnAnimationEvent_Impact()
    {
        if (IsDead) return;

        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(1, -1).normalized,
            new Vector2(-1, 1).normalized, new Vector2(-1, -1).normalized
        };

        foreach (var dir in directions)
        {
            var orb = GamePools.Hitboxes.Get(darkOrbPrefab, transform.position, Quaternion.identity);
            orb.gameObject.SetActive(true);

            if (orb is ProjectileAttackObject projectile)
            {
                projectile.speed = orbSpeed;
            }

            orb.Initialize(damage, LayerMask.GetMask("Player"), dir);
        }
    }

    // 2. Вызывается Unity Animator'ом в конце анимации
    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        _curCooldown = attackCooldown;
        Brain.ChangeState(new ES_IdleState());
    }
}