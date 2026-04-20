using System;
using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class Entity : InjectMonoBehaviour
{
  private const float ShellDistance = 0.01f;

  [Header("Stats")]
  [SerializeField]
  private Stat health;

  [SerializeField]
  private ModifiableStat maxHealth;

  [SerializeField]
  private ModifiableStat moveSpeed;

  [SerializeField]
  private ModifiableStat dodgeChance;

  [Header("Target")]
  public Transform target;

  private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
  private ContactFilter2D _contactFilter;
  private Collider2D _col;
  private Rigidbody2D _rb;

  // События для визуализации и систем
  public event Action<float> OnTakeDamage;
  public event Action OnDeath;
  public event Action OnAppear;
  public event Action<Vector2> OnMove; // Для аниматора

  public virtual float Health
  {
    get => health.Value;
    set => health.Value = Mathf.Clamp(value, 0, MaxHealth);
  }
  public virtual float MaxHealth => maxHealth.ModifiedValue;
  public bool IsDead { get; protected set; }
  public float MoveSpeed => moveSpeed.ModifiedValue;
  public bool IsDashing { get; private set; }

  public int InvulnerabilityProcCount { get; set; } = 0;
  public bool IsInvulnerable => InvulnerabilityProcCount > 0;

  [Header("Visuals")]
  [SerializeField] protected Animator _animator;
  [SerializeField] protected SpriteRenderer _spriteRenderer;

  protected readonly int animSpeed = Animator.StringToHash("Speed");
  protected readonly int animHit = Animator.StringToHash("Hit");
  protected readonly int animDie = Animator.StringToHash("Die");
  protected readonly int animAttack = Animator.StringToHash("Attack");

  protected virtual void Update()
  {
      if (IsDead || _animator == null) return;

      // Автоматически передаем скорость в Аниматор. 
      // В Animator Controller нужно настроить переход из Idle в Run, если Speed > 0.01
      _animator.SetFloat(animSpeed, CurrentMoveDirection.sqrMagnitude);

      HandleFlip(CurrentMoveDirection);
  }

  // Универсальный метод поворота
  public virtual void HandleFlip(Vector2 direction)
  {
      if (_spriteRenderer == null) return;

      // Используем порог 0.01f, чтобы не флипать из-за микро-колебаний джойстика
      if (direction.x > 0.01f)
      {
          _spriteRenderer.flipX = false; // Смотрит вправо (если исходный спрайт нарисован вправо)
      }
      else if (direction.x < -0.01f)
      {
          _spriteRenderer.flipX = true;  // Смотрит влево
      }
  }

  public virtual void Attack(Vector2 direction)
  {
      // Когда сущность атакует, она должна резко повернуться в сторону атаки!
      HandleFlip(direction);
  }

  protected virtual void Awake()
  {
    _rb = GetComponent<Rigidbody2D>();
    _col = GetComponent<Collider2D>();

    _contactFilter.useTriggers = false;
    _contactFilter.SetLayerMask(LayerMask.GetMask("Obstacle"));
    _contactFilter.useLayerMask = true;
  }

  protected virtual void Start()
  {
    Health = MaxHealth;
    IsDead = false;
  }

  protected void TargetTo(Transform _target)
  {
    target = _target;
  }

  public virtual void TakeDamage(float amount)
  {
    if (IsDead || IsInvulnerable || amount <= 0)
      return;

    var hasDodged = UnityEngine.Random.value <= (dodgeChance.ModifiedValue);
    if (hasDodged)
    {
      OnTakeDamage?.Invoke(0f);
      return;
    }

    Health -= amount;
    OnTakeDamage?.Invoke(amount);

    if (_animator != null) _animator.SetTrigger(animHit);

    if (Health <= 0)
      Die();
  }

  protected virtual void Die()
  {
    if (IsDead)
      return;
    IsDead = true;
    Health = 0;
    OnDeath?.Invoke();
    _col.enabled = false;

    if (_animator != null) _animator.SetTrigger(animDie);
  }

  public virtual void ResetEntity()
  {
    IsDead = false;
    _col.enabled = true;
    Health = MaxHealth;
    InvulnerabilityProcCount = 0; // Сбрасываем неуязвимость
    OnAppear?.Invoke();
  }

  public Vector2 CurrentMoveDirection { get; private set; }

  public void Move(Vector2 direction, float speedBoost = 1)
  {
    // Если мертв — обнуляем направление и выходим
    if (IsDead)
    {
      CurrentMoveDirection = Vector2.zero;
      return;
    }

    // Сохраняем направление (даже если оно нулевое)
    CurrentMoveDirection = direction;

    if (direction.sqrMagnitude < 0.001f)
      return;

    // Вызываем событие (оно у тебя уже было в коде, теперь мы его реально используем)
    OnMove?.Invoke(direction);

    var deltaMove = direction * MoveSpeed * Time.fixedDeltaTime * speedBoost;

    ResolveOverlap(); // проверка уже внутри стены

    var maxIterations = 4;
    for (var i = 0; i < maxIterations; i++)
    {
      var distance = deltaMove.magnitude;
      if (distance < 0.0001f)
        break;

      var count = _col.Cast(
        deltaMove.normalized,
        _contactFilter,
        _hitBuffer,
        distance + ShellDistance
      );

      if (count > 0)
      {
        var hit = _hitBuffer[0];

        var safeDistance = Mathf.Max(0, hit.distance - ShellDistance);
        _rb.position += deltaMove.normalized * safeDistance;

        var remainingDelta = deltaMove.normalized * (distance - safeDistance);
        deltaMove = remainingDelta - Vector2.Dot(remainingDelta, hit.normal) * hit.normal;

        if (Vector2.Dot(deltaMove, direction) <= 0)
          deltaMove = Vector2.zero;
      }
      else
      {
        _rb.position += deltaMove;
        break;
      }
    }
  }

  private void ResolveOverlap()
  {
    var results = new Collider2D[5];
    var count = _col.Overlap(_contactFilter, results);

    for (var i = 0; i < count; i++)
    {
      var dist = _col.Distance(results[i]);
      if (dist.isOverlapped)
        _rb.position += dist.normal * dist.distance;
    }
  }

  public void Dash(Vector2 direction, float distance, float duration)
  {
    if (!IsDashing)
      StartCoroutine(DashCoroutine(direction, distance, duration));
  }

  protected virtual IEnumerator DashCoroutine(Vector2 direction, float distance, float duration)
  {
    IsDashing = true;
    float elapsed = 0;
    while (elapsed < duration)
    {
      Move(direction, distance);
      elapsed += Time.fixedDeltaTime;
      yield return new WaitForFixedUpdate();
    }
    IsDashing = false;
  }

  public virtual Stat GetStat(StatName statName) {
    switch (statName)
    {
      case StatName.MaxHealth: return maxHealth;
      case StatName.DodgeChance: return dodgeChance;
      case StatName.MoveSpeed: return moveSpeed;
      case StatName.Health: return health;
      default: return null;
    }
  }

  public virtual void OnAnimationEvent_Impact() { }

  // Вызывается в самом конце анимации атаки
  public virtual void OnAnimationEvent_End() { }
}
