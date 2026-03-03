using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordWeapon", menuName = "Inside-Ventura/Artifacts/Weapon/Sword")]
public class SwordWeapon : Weapon {
  [Header("Attack")] [SerializeField] private float normalAngle = 90f; // угол сектора атаки в градусах

  [SerializeField] private float normalRange = 2f;
  [SerializeField] private LayerMask enemyLayer;

  [Header("Special Attack")] [SerializeField]
  private float specialAngle = 30f;

  [SerializeField] private float specialRange = 5f;
  [SerializeField] private float specialDamageMultiplier = 2f;
  [SerializeField] private float lungeDistance = 15f;

  [Header("Debug")] [SerializeField] private bool drawSector = true;

  [SerializeField] private float debugDuration = 0.2f;
  [SerializeField] private Color normalColor = Color.yellow;
  [SerializeField] private Color specialColor = Color.red;

  private void OnEnable() {
    enemyLayer = LayerMask.GetMask("Entity");
  }

  public override void Attack(GameObject playerObject, Vector2 direction) {
    var player = playerObject.GetComponent<Player>();

    if (!CanAttack()) return;
    UpdateCombo();

    var isSpecial = currentChainCount == chainCount;

    var angle = isSpecial ? specialAngle : normalAngle;
    var range = isSpecial ? specialRange : normalRange;
    var damageAmount = isSpecial ? damage * specialDamageMultiplier : damage;

    Vector2 playerPosition = playerObject.transform.position;
    var dir = direction.normalized;

    // поиск врагов в секторе
    var hits = Physics2D.OverlapCircleAll(playerPosition, range, enemyLayer);
    foreach (var hit in hits) {
      Vector2 toEnemy = (hit.transform.position - (Vector3)playerPosition).normalized;
      var angleBetween = Vector2.Angle(dir, toEnemy);
      if (angleBetween <= angle / 2f) {
        var enemy = hit.GetComponent<Enemy>();
        if (enemy != null) {
          Debug.Log($"Удар по {enemy.name} с уроном {damageAmount}!");

          enemy.TakeDamage((int)damageAmount);
        }
      }
    }

    if (isSpecial) player.Move(dir * lungeDistance);

    // рисуем сектор дипсиком
    if (drawSector)
      DrawSector(playerPosition, dir, angle, range, isSpecial ? specialColor : normalColor, debugDuration);

    lastAttackTime = Time.time; // время удара
  }

  private void DrawSector(Vector2 center, Vector2 direction, float angle, float radius, Color color, float duration) {
    var segments = 20;
    var halfAngle = angle * 0.5f * Mathf.Deg2Rad;
    var startAngle = -halfAngle;
    var endAngle = halfAngle;

    var prevPoint = center + direction.Rotate(startAngle) * radius;
    for (var i = 1; i <= segments; ++i) {
      var t = (float)i / segments;
      var currentAngle = Mathf.Lerp(startAngle, endAngle, t);
      var point = center + direction.Rotate(currentAngle) * radius;
      Debug.DrawLine(prevPoint, point, color, duration);
      prevPoint = point;
    }

    Debug.DrawLine(center, center + direction.Rotate(startAngle) * radius, color, duration);
    Debug.DrawLine(center, center + direction.Rotate(endAngle) * radius, color, duration);
  }
}

public static class Vector2Extensions {
  public static Vector2 Rotate(this Vector2 v, float radians) {
    var sin = Mathf.Sin(radians);
    var cos = Mathf.Cos(radians);
    return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
  }
}
