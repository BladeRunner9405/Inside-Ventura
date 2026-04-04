using UnityEngine;

public class SectorAttackObject : AttackObject
{
  [Header("Debug Settings")]
  [SerializeField]
  private bool showDebugVisuals = true;

  [SerializeField]
  private float debugLineDuration = 1.5f;

  private readonly Collider2D[] _hitBuffer = new Collider2D[32];

  private float _angle;
  private float _radius;

  // Уникальный инициализатор для сектора (т.к. нужны угол и радиус)
  public void Initialize(
    float damage,
    LayerMask layer,
    Vector2 direction,
    float angle,
    float radius
  )
  {
    _angle = angle;
    _radius = radius;

    // Вызываем базовый Initialize, который сам пнет Аниматор и вызовет PerformAttack
    base.Initialize(damage, layer, direction);

    if (showDebugVisuals)
      DrawSectorDebug();
  }

  // Этот метод вызовется автоматически из базового класса на кадре урона!
  protected override void PerformAttack()
  {
    var count = Physics2D.OverlapCircleNonAlloc(
      transform.position,
      _radius,
      _hitBuffer,
      targetLayer
    );

    for (var i = 0; i < count; i++)
    {
      var col = _hitBuffer[i];
      Vector2 toTarget = (col.transform.position - transform.position).normalized;

      if (Vector2.Angle(Direction, toTarget) <= _angle / 2f)
      {
        if (showDebugVisuals)
          Debug.DrawLine(
            transform.position,
            col.transform.position,
            Color.green,
            debugLineDuration
          );

        TryDealDamage(col);
      }
      else
      {
        if (showDebugVisuals)
          Debug.DrawLine(transform.position, col.transform.position, Color.red, debugLineDuration);
      }
    }
  }

  private void DrawSectorDebug()
  {
    var pos = transform.position;
    // ИСПОЛЬЗУЕМ Direction ИЗ БАЗОВОГО КЛАССА
    var rightLimit = Quaternion.Euler(0, 0, -_angle / 2f) * Direction * _radius;
    var leftLimit = Quaternion.Euler(0, 0, _angle / 2f) * Direction * _radius;

    Debug.DrawRay(pos, rightLimit, Color.yellow, debugLineDuration);
    Debug.DrawRay(pos, leftLimit, Color.yellow, debugLineDuration);

    var segments = 10;
    var angleStep = _angle / segments;
    var prevPoint = pos + (Vector3)rightLimit;

    for (var i = 1; i <= segments; i++)
    {
      var nextDir = Quaternion.Euler(0, 0, -_angle / 2f + angleStep * i) * Direction;
      var nextPoint = pos + (Vector3)(nextDir * _radius);

      Debug.DrawLine(prevPoint, nextPoint, Color.yellow, debugLineDuration);
      prevPoint = nextPoint;
    }
  }
}
