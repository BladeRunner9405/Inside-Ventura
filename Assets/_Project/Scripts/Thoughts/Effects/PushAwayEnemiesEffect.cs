using UnityEngine;

[CreateAssetMenu(fileName = "PushAwayEnemiesEffect", menuName = "Inside-Ventura/Effects/PushAwayEnemiesEffect")]
public class PushAwayEnemiesEffect : Effect {
  [SerializeField] private float pushRadius = 3f;
  [SerializeField] private float pushDistance = 5f;
  [SerializeField] private float pushDuration = 0.5f;

  private Accessory _accessory;

  public override void OnEquipThought(Artifact artifact) {
    if (artifact is Accessory accessory) {
      _accessory = accessory;
      _accessory.OnAbilityUsed += OnAccessoryUsed;
      Debug.Log("Отталкивание врагов подписалось на применение аксессуара");
    }
  }

  public override void OnUnequipThought(Artifact artifact) {
    if (_accessory != null) {
      _accessory.OnAbilityUsed -= OnAccessoryUsed;
      _accessory = null;
      Debug.Log("Отталкивание врагов отписалось от применения аксессуара");
    }
  }

  private void OnAccessoryUsed(Vector2 direction) {
    var player = _accessory.PlayerAccessor.Player;

    Vector2 playerPos = player.transform.position;
    var colliders = Physics2D.OverlapCircleAll(playerPos, pushRadius, LayerMask.GetMask("Enemy"));

    foreach (var col in colliders) {
      var enemy = col.GetComponent<Enemy>();
      if (enemy != null && !enemy.IsDead) {
        Debug.Log("Отталкиваем Enemy");
        var pushDirection = ((Vector2)enemy.transform.position - playerPos).normalized;

        if (pushDirection == Vector2.zero) pushDirection = Vector2.right;

        enemy.Dash(pushDirection, pushDistance, pushDuration);
      }
    }
  }
}
