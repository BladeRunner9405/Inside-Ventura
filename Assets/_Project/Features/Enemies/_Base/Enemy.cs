using System.Collections;
using System.Collections.Generic;
using CherryFramework.DependencyManager;
using InsideVentura.AI; // Не забываем namespace
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))] // Гарантируем наличие мозга
public class Enemy : Entity {
  [Header("Enemy Base Stats")] public float damage;
  public bool isBoss;

  [Header("Context Steering Settings")] [SerializeField]
  private float detectionRadius = 1.5f;

  [SerializeField] private LayerMask obstacleLayer;

  [SerializeField] private LayerMask enemyLayer; // Слой врагов

  [SerializeField, Range(1, 16)] private int raysCount = 8;

  [Header("Movement Smoothing")] [SerializeField]
  private float steeringLerpSpeed = 10f;

  private Vector2 _currentSteeringVelocity;

  [Header("UI")] [SerializeField] private FloatingDamage floatingDamagePrefab;

  [Inject] protected PlayerAccessor PlayerAccessor;

  [SerializeField] protected EnemyBrain Brain;

  private Vector2[] _rayDirections;
  private float[] _interest;
  private float[] _danger;

  protected override void Awake() {
    base.Awake();
    InitializeSteering();
  }

  protected override void Start() {
    base.Start();
    TargetTo(PlayerAccessor.Transform);
  }

  private void InitializeSteering() {
    _rayDirections = new Vector2[raysCount];
    _interest = new float[raysCount];
    _danger = new float[raysCount];

    for (int i = 0; i < raysCount; i++) {
      float angle = i * 2 * Mathf.PI / raysCount;
      _rayDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
  }

  public void MoveWithSteering(Vector2 targetDirection) {
    if (targetDirection == Vector2.zero) {
      Move(Vector2.zero);
      _currentSteeringVelocity = Vector2.zero;
      return;
    }

    // 1. Сброс карт
    for (int i = 0; i < raysCount; i++) {
      _interest[i] = 0;
      // Мягко гасим старую опасность
      _danger[i] = Mathf.MoveTowards(_danger[i], 0, Time.deltaTime * 5f);
    }

    // 2. Опасности (Стены)
    // Уменьшим множитель выноса лучей вперед (с 0.5 до 0.2), чтобы лучи не рождались ВНУТРИ стен
    Vector2 rayOrigin = (Vector2)transform.position + (_currentSteeringVelocity * 0.2f);

    for (int i = 0; i < raysCount; i++) {
      RaycastHit2D hit = Physics2D.Raycast(
        rayOrigin,
        _rayDirections[i],
        detectionRadius,
        obstacleLayer | enemyLayer
      );

      bool isHit = hit.collider != null;

      // --- ВОЗВРАЩАЕМ ЛУЧИ ДЛЯ ДЕБАГА ---
      Color debugColor = isHit ? Color.red : Color.white;
      Debug.DrawRay(rayOrigin, _rayDirections[i] * detectionRadius, debugColor);

      if (isHit) {
        _danger[i] = 1.0f - (hit.distance / detectionRadius);
      }
    }

    // 3. Интересы (Цель)
    for (int i = 0; i < raysCount; i++) {
      float dot = Vector2.Dot(targetDirection.normalized, _rayDirections[i]);
      _interest[i] = Mathf.Max(0, dot);
    }

    // 4. Подавление (Обход)
    for (int i = 0; i < raysCount; i++) {
      // Порог 0.3f значит, что если стена занимает 30% дистанции луча,
      // мы уже перестаем хотеть туда идти.
      if (_danger[i] > 0.3f) {
        _interest[i] = 0;
      }
    }

    // 5. Итоговый вектор
    Vector2 outputDirection = Vector2.zero;
    for (int i = 0; i < raysCount; i++) {
      outputDirection += _rayDirections[i] * (_interest[i] - _danger[i] * 1.5f);
    }

    // 6. Плавность
    if (outputDirection.sqrMagnitude > 0.01f) {
      outputDirection.Normalize();
    }
    else {
      outputDirection = Vector2.zero;
    }

    _currentSteeringVelocity = Vector2.Lerp(
      _currentSteeringVelocity,
      outputDirection,
      Time.deltaTime * steeringLerpSpeed
    );

    Move(_currentSteeringVelocity);
  }

  public override void ResetEntity() {
    base.ResetEntity();
    Brain.ResetBrain();

    // Находим аниматор и глушим незаконченные атаки!

    _view.ResetToIdle();
  }

  public override void TakeDamage(float amount) {
    base.TakeDamage(amount);

    var damageText = GamePools.FloatingDamages.Get(floatingDamagePrefab, transform.position, Quaternion.identity);
    damageText.gameObject.SetActive(true);
    damageText.Initialize(amount);
  }
}
