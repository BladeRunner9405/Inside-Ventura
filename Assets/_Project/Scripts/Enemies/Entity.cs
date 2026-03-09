using System;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class Entity : InjectMonoBehaviour {
  private const float _shellDistance = 0.01f; // отступ, чтобы не врастать в стены
  [SerializeField] private int health;
  [SerializeField] private int maxHealth = 100;
  [SerializeField] private bool isDead;

  [SerializeField] private bool isInvulnerable;

  public Transform target; // Transform, на кого смотрит Entity

  public float moveSpeed = 5f;
  private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];

  private ContactFilter2D _contactFilter;
  protected Collider2D col;
  protected Rigidbody2D rb;

  public int Health {
    get => health;
    protected set => health = Mathf.Clamp(value, 0, MaxHealth);
  }

  public int MaxHealth {
    get => maxHealth;
    protected set {
      maxHealth = Mathf.Max(1, value);
      if (health > maxHealth)
        health = maxHealth;
    }
  }

  public bool IsDead => isDead;

  protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();

    _contactFilter.useTriggers = false;
    _contactFilter.SetLayerMask(LayerMask.GetMask("Obstacle"));
    _contactFilter.useLayerMask = true;

    health = maxHealth;
    isDead = false;
  }

  public void SetInvulnerable(bool invulnerable) {
    isInvulnerable = invulnerable;
  }

  private event Action<int> OnTakeDamage;
  public event Action OnDeath;

  public void Move(Vector2 direction) {
    if (direction.sqrMagnitude < 0.001f) return;

    var deltaMove = direction * moveSpeed * Time.fixedDeltaTime;

    ResolveOverlap(); // проверка уже внутри стены

    var maxIterations = 4;
    for (var i = 0; i < maxIterations; i++) {
      var distance = deltaMove.magnitude;
      if (distance < 0.0001f) break;

      var count = col.Cast(deltaMove.normalized, _contactFilter, _hitBuffer, distance + _shellDistance);

      if (count > 0) {
        var hit = _hitBuffer[0];

        var safeDistance = Mathf.Max(0, hit.distance - _shellDistance);
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

  // обрезает вектор до столкновения со стеной
  protected float CalculateSafeDistance(Vector2 direction, float distance) {
    var count = col.Cast(direction, _contactFilter, _hitBuffer, distance + _shellDistance);

    if (count > 0) {
      var hit = _hitBuffer[0];

      var safeDistance = Mathf.Max(0, hit.distance - _shellDistance);
      return safeDistance;
    }

    return distance;
  }

  public void Attack(Player player) {
    // ...
  }

  public void TakeDamage(int amount) {
    if (IsDead || isInvulnerable) return;
    if (amount <= 0) return;

    Health -= amount;

    OnTakeDamage?.Invoke(amount);

    if (Health == 0)
      Die();

    Debug.Log($"{gameObject.name} получил {amount} урона. Его здоровье - {Health}/{maxHealth}");
  }

  public void Die() {
    if (IsDead) return;

    isDead = true;
    Health = 0;
    OnDeath?.Invoke();
  }

  public void TargetTo(Transform target) // назначить новую цель
  {
    this.target = target;
  }
}
