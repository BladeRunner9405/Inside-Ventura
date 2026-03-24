using System;
using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Entity : InjectMonoBehaviour {
  private const float ShellDistance = 0.01f; // отступ, чтобы не врастать в стены

  [Header("Health")] [SerializeField] private DynamicStat health = new();
  [SerializeField] private ModifiableStat maxHealth = new(100f);
  [SerializeField] private ModifiableStat dodgeChance = new();

  [Header("Moving")] [SerializeField] private ModifiableStat moveSpeed = new(5f);

  [Header("Target")] public Transform target; // Transform, на кого смотрит Entity

  // переменные для перемещения без NavMesh
  private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
  private ContactFilter2D _contactFilter;

  protected Collider2D col;

  private bool isInvulnerable;
  protected Rigidbody2D rb;

  public float Health {
    get => health.Value;
    set => health.Value = Mathf.Clamp(value, 0, MaxHealth);
  }

  public float MaxHealth => maxHealth.ModifiedValue;

  public bool IsDead { get; private set; }

  public float DodgeChance => Mathf.Min(1f, dodgeChance.ModifiedValue);

  public float MoveSpeed {
    get => moveSpeed.ModifiedValue;
    set => moveSpeed.Value = value;
  }

  public bool IsDashing { get; private set; }

  protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();

    Health = MaxHealth;
    IsDead = false;

    _contactFilter.useTriggers = false;
    _contactFilter.SetLayerMask(LayerMask.GetMask("Obstacle"));
    _contactFilter.useLayerMask = true;
  }

  public Stat GetStat(StatName statName) {
    if (statName == StatName.MaxHealth)
      return maxHealth;
    if (statName == StatName.DodgeChance)
      return dodgeChance;
    if (statName == StatName.MoveSpeed)
      return moveSpeed;
    if (statName == StatName.Health)
      return health;
    return null;
  }

  public void SetInvulnerable(bool invulnerable) {
    isInvulnerable = invulnerable;
  }

  private event Action<float> OnTakeDamage;
  public event Action OnDeath;

  public void TakeDamage(float amount) {
    if (IsDead || isInvulnerable) return;
    if (amount <= 0) return;

    var hasDodged = Random.value <= DodgeChance;
    var finalAmount = hasDodged ? 0f : amount;

    Health -= finalAmount;

    OnTakeDamage?.Invoke(finalAmount);

    if (Health == 0)
      Die();

    Debug.Log($"{gameObject.name} получил {finalAmount} урона. Его здоровье - {Health}/{MaxHealth}");
  }

  public void Die() {
    if (IsDead) return;

    // GetComponent<SpriteRenderer>().enabled = false; // заглушка

    IsDead = true;
    Health = 0;
    OnDeath?.Invoke();
  }

  public void TargetTo(Transform target) // назначить новую цель
  {
    this.target = target;
  }

  public void Move(Vector2 direction) {
    if (direction.sqrMagnitude < 0.001f) return;

    var deltaMove = direction * MoveSpeed * Time.fixedDeltaTime;

    ResolveOverlap(); // проверка уже внутри стены

    var maxIterations = 4;
    for (var i = 0; i < maxIterations; i++) {
      var distance = deltaMove.magnitude;
      if (distance < 0.0001f) break;

      var count = col.Cast(deltaMove.normalized, _contactFilter, _hitBuffer, distance + ShellDistance);

      if (count > 0) {
        var hit = _hitBuffer[0];

        var safeDistance = Mathf.Max(0, hit.distance - ShellDistance);
        rb.position += deltaMove.normalized * safeDistance;

        var remainingDelta = deltaMove.normalized * (distance - safeDistance);
        deltaMove = remainingDelta - Vector2.Dot(remainingDelta, hit.normal) * hit.normal;

        if (Vector2.Dot(deltaMove, direction) <= 0) deltaMove = Vector2.zero;
      }
      else {
        rb.position += deltaMove;
        break;
      }
    }
  }

  private void ResolveOverlap() {
    var results = new Collider2D[5];
    var count = col.Overlap(_contactFilter, results);

    for (var i = 0; i < count; i++) {
      var dist = col.Distance(results[i]);
      if (dist.isOverlapped) rb.position += dist.normal * dist.distance;
    }
  }

  public void Dash(Vector2 direction, float distance, float duration) {
    if (direction == Vector2.zero) direction = Vector2.right;
    if (IsDashing) return;

    StartCoroutine(DashCoroutine(direction, distance, duration));
  }

  protected virtual IEnumerator DashCoroutine(Vector2 direction, float distance, float duration) {
    IsDashing = true;
    var originalSpeed = MoveSpeed;
    MoveSpeed = distance / duration;

    SetInvulnerable(true);

    var elapsed = 0f;
    while (elapsed < duration) {
      Move(direction);

      elapsed += Time.fixedDeltaTime;

      yield return new WaitForFixedUpdate();
    }

    MoveSpeed = originalSpeed;
    SetInvulnerable(false);
    IsDashing = false;

    ResolveOverlap();
  }
}
