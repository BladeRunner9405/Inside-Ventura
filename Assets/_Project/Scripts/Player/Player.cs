using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;

// Не забудьте добавить, так как используется .DOMove и .SetEase

public class Player : Entity {
  private const float ShellDistance = 0.01f; // отступ, чтобы не врастать в стены
  [SerializeField] private ItemPickup itemPickup;

  [SerializeField] private PlayerInventory inventory;
  [SerializeField] private PlayerEquipment equipment;
  [SerializeField] private PlayerStats stats;
  private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
  private ContactFilter2D _contactFilter;

  [Inject] private PlayerAccessor _playerAccessor;
  public PlayerInventory Inventory => inventory;
  public PlayerEquipment Equipment => equipment;
  public PlayerStats Stats => stats;

  public bool IsDashing { get; private set; }

  protected override void Awake() {
    base.Awake();

    _contactFilter.useTriggers = false;
    _contactFilter.SetLayerMask(LayerMask.GetMask("Obstacle"));
    _contactFilter.useLayerMask = true;
  }

  protected override void OnEnable() {
    base.OnEnable();
    _playerAccessor.RegisterPlayer(this);
  }

  private void OnDisable() {
    _playerAccessor.UnregisterPlayer(this);
  }

  public void TryToInteract() {
    itemPickup.TryToInteract();
  }

  public void Dash(Vector2 direction, float distance, float duration) {
    if (direction == Vector2.zero) direction = Vector2.right;
    if (IsDashing) return;

    StartCoroutine(DashCoroutine(direction, distance, duration));
  }

  private IEnumerator DashCoroutine(Vector2 direction, float distance, float duration) {
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

  // обрезает вектор до столкновения со стеной
  protected float CalculateSafeDistance(Vector2 direction, float distance) {
    var count = col.Cast(direction, _contactFilter, _hitBuffer, distance + ShellDistance);

    if (count > 0) {
      var hit = _hitBuffer[0];

      var safeDistance = Mathf.Max(0, hit.distance - ShellDistance);
      return safeDistance;
    }

    return distance;
  }

  /*void Heal(int amount) {
      if (IsDead) return;
      if (amount <= 0) return;

      Health += amount;
  }

  void ModifyMaxHealth(int delta) {
    if (IsDead) return;

    MaxHealth += delta;
  }*/
}
