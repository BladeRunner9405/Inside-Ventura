using CherryFramework.DependencyManager;
using UnityEngine;

public class Enemy : Entity {
  public int damage;
  public bool isBoss;

  [Inject] private PlayerAccessor _playerAccessor;

  protected void Start() {
    var player = _playerAccessor.Player;
    if (!player) {
      Debug.LogWarning("Игрок не найден и не назначен врагу.", this);
      return;
    }

    TargetTo(player.transform);
  }

  private void FixedUpdate() {
    if (target && !IsDead) Move((target.position - transform.position).normalized);
  }
}
