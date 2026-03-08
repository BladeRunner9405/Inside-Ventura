using CherryFramework.DependencyManager;
using DG.Tweening;
using UnityEngine;

public class Player : Entity {
  [SerializeField] private ItemPickup itemPickup;

  [Inject] private PlayerAccessor _playerAccessor;

  protected override void OnEnable() {
    base.OnEnable();
    _playerAccessor?.RegisterPlayer(this);
  }

  private void OnDisable() {
    _playerAccessor?.UnregisterPlayer(this);
  }

  public void TryToInteract() {
    itemPickup.TryToInteract();
  }

  public void Dash(Vector2 direction, float distance, float duration) {
    if (direction == Vector2.zero) direction = Vector2.right;

    var startPos = transform.position;
    var originalDistance = distance;
    var actualDistance = CalculateSafeDistance(direction, originalDistance);

    // если упёрлись в стену
    if (actualDistance <= 0f)
      return;

    var targetPos = startPos + (Vector3)direction * actualDistance;

    var actualDuration = duration * (actualDistance / originalDistance);

    SetInvulnerable(true);

    // сам дэш, Ease.OutQuad - анимация начинается быстро и замедляется к концу
    transform.DOMove(targetPos, actualDuration)
      .SetEase(Ease.OutQuad)
      .OnComplete(() => SetInvulnerable(false));
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
