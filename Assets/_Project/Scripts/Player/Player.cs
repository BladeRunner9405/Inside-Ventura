using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;

public class Player : Entity {
  [SerializeField] private ItemPickup itemPickup;

  [SerializeField] private PlayerInventory inventory;

  [Inject] private PlayerAccessor _playerAccessor;
  public PlayerInventory Inventory => inventory;
  public bool IsDashing { get; private set; }

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
    var originalSpeed = moveSpeed;
    moveSpeed = distance / duration;

    SetInvulnerable(true);

    var elapsed = 0f;
    while (elapsed < duration) {
      Move(direction);

      elapsed += Time.fixedDeltaTime;

      yield return new WaitForFixedUpdate();
    }

    moveSpeed = originalSpeed;
    SetInvulnerable(false);
    IsDashing = false;

    ResolveOverlap();
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
