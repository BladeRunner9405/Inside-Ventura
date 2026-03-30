using UnityEngine;

public class SectorAttackObject : AttackObject {
  [Header("Debug Settings")]
  [Tooltip("Рисовать ли сектор и линии попаданий в Scene View во время игры?")]
  [SerializeField]
  private bool showDebugVisuals = true;

  [SerializeField] private float debugLineDuration = 1.5f;

  private readonly Collider2D[] _hitBuffer = new Collider2D[32];

  private float _angle;
  private Vector2 _direction;
  private float _radius;

  protected override void Update() {
    base.Update();
    if (lifeTime > 0) CheckSectorHits();
  }

  public void Initialize(float damage, LayerMask layer, float timeToLive, float angle, float radius,
    Vector2 direction) {
    base.Initialize(damage, layer, timeToLive);

    _angle = angle;
    _radius = radius;
    _direction = direction.normalized;

    if (showDebugVisuals) DrawSectorDebug();

    CheckSectorHits();
  }

  private void CheckSectorHits() {
    var count = Physics2D.OverlapCircleNonAlloc(transform.position, _radius, _hitBuffer, targetLayer);

    for (var i = 0; i < count; i++) {
      var col = _hitBuffer[i];
      Vector2 toTarget = (col.transform.position - transform.position).normalized;

      if (Vector2.Angle(_direction, toTarget) <= _angle / 2f) {
        if (showDebugVisuals)
          // Зеленая линия = цель внутри сектора
          Debug.DrawLine(transform.position, col.transform.position, Color.green, debugLineDuration);

        TryDealDamage(col);
      }
      else {
        if (showDebugVisuals)
          // Красная линия = цель рядом, но вне угла атаки
          Debug.DrawLine(transform.position, col.transform.position, Color.red, debugLineDuration);
      }
    }
  }

  // Рисование
  private void DrawSectorDebug() {
    var pos = transform.position;

    var rightLimit = Quaternion.Euler(0, 0, -_angle / 2f) * _direction * _radius;
    var leftLimit = Quaternion.Euler(0, 0, _angle / 2f) * _direction * _radius;

    Debug.DrawRay(pos, rightLimit, Color.yellow, debugLineDuration);
    Debug.DrawRay(pos, leftLimit, Color.yellow, debugLineDuration);

    var segments = 10;
    var angleStep = _angle / segments;
    var prevPoint = pos + rightLimit;

    for (var i = 1; i <= segments; i++) {
      var nextDir = Quaternion.Euler(0, 0, -_angle / 2f + angleStep * i) * _direction;
      var nextPoint = pos + nextDir * _radius;

      Debug.DrawLine(prevPoint, nextPoint, Color.yellow, debugLineDuration);
      prevPoint = nextPoint;
    }
  }
}
