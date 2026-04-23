using System.Collections;
using System.Collections.Generic;
using CherryFramework.DependencyManager;
using InsideVentura.AI;
using UnityEngine;

/// <summary>
/// Базовый класс врага с системой Context Steering и управлением атаками.
/// </summary>
[RequireComponent(typeof(EnemyBrain))]
public class Enemy : Entity 
{
    [Header("Enemy Base Stats")] 
    public float damage;
    public bool isBoss;

    [Header("Base Attack Settings")]
    public float attackCooldown = 1f; // Универсальный кулдаун для всех врагов
    
    [Header("Context Steering Settings")] 
    [SerializeField] private float detectionRadius = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask enemyLayer; // Слой врагов
    [SerializeField, Range(1, 16)] private int raysCount = 8;

    [Header("Movement Smoothing")] 
    [SerializeField] private float steeringLerpSpeed = 10f;

    [Header("UI")] 
    [SerializeField] private FloatingDamage floatingDamagePrefab;

    [Inject] protected PlayerAccessor PlayerAccessor;

    [SerializeField] protected EnemyBrain Brain;

    // --- ПЕРЕМЕННЫЕ СОСТОЯНИЯ ---
    protected float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;
    protected Vector2 _attackDir; // Направление последней атаки

    private Vector2[] _rayDirections;
    private float[] _interest;
    private float[] _danger;
    private Vector2 _currentSteeringVelocity;

    // --- ИНИЦИАЛИЗАЦИЯ ---

    protected override void Awake() 
    {
        base.Awake();
        InitializeSteering();
    }

    protected override void Start() 
    {
        base.Start();
        // Устанавливаем цель для Entity (базовая логика)
        if (PlayerAccessor != null)
            TargetTo(PlayerAccessor.Transform);
    }

    private void InitializeSteering() 
    {
        _rayDirections = new Vector2[raysCount];
        _interest = new float[raysCount];
        _danger = new float[raysCount];

        for (int i = 0; i < raysCount; i++) 
        {
            float angle = i * 2 * Mathf.PI / raysCount;
            _rayDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    // --- ОБНОВЛЕНИЕ ---

    protected override void Update()
    {
        base.Update(); // Важно: там вызывается Flip() и расчет Speed для аниматора
        
        if (IsDead) return;
        
        // Тикаем кулдаун атаки
        if (_curCooldown > 0)
            _curCooldown -= Time.deltaTime;
    }

    // --- ЛОГИКА ДВИЖЕНИЯ (CONTEXT STEERING) ---

    public void MoveWithSteering(Vector2 targetDirection) 
    {
        if (targetDirection == Vector2.zero) 
        {
            Move(Vector2.zero);
            _currentSteeringVelocity = Vector2.zero;
            return;
        }

        // 1. Сброс карт интересов и опасностей
        for (int i = 0; i < raysCount; i++) 
        {
            _interest[i] = 0;
            // Мягко гасим старую опасность для плавности
            _danger[i] = Mathf.MoveTowards(_danger[i], 0, Time.deltaTime * 5f);
        }

        // 2. Определение опасностей (Стены и другие враги)
        // Смещение точки начала лучей чуть вперед, чтобы не застревать внутри коллайдеров
        Vector2 rayOrigin = (Vector2)bodyCollider.bounds.center + (_currentSteeringVelocity * 0.2f);

        for (int i = 0; i < raysCount; i++) 
        {
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                _rayDirections[i],
                detectionRadius,
                obstacleLayer | enemyLayer
            );

            bool isHit = hit.collider != null;

            // Визуализация лучей в редакторе
            Color debugColor = isHit ? Color.red : Color.white;
            Debug.DrawRay(rayOrigin, _rayDirections[i] * detectionRadius, debugColor);

            if (isHit) 
            {
                _danger[i] = 1.0f - (hit.distance / detectionRadius);
            }
        }

        // 3. Определение интересов (Куда хотим идти - к цели)
        for (int i = 0; i < raysCount; i++) 
        {
            float dot = Vector2.Dot(targetDirection.normalized, _rayDirections[i]);
            _interest[i] = Mathf.Max(0, dot);
        }

        // 4. Подавление интересов опасностями
        for (int i = 0; i < raysCount; i++) 
        {
            // Если опасность значительна (> 30% дистанции), обнуляем интерес к этому направлению
            if (_danger[i] > 0.3f) 
            {
                _interest[i] = 0;
            }
        }

        // 5. Расчет итогового вектора
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < raysCount; i++) 
        {
            outputDirection += _rayDirections[i] * (_interest[i] - _danger[i] * 1.5f);
        }

        // 6. Нормализация и плавное затухание
        if (outputDirection.sqrMagnitude > 0.01f) 
        {
            outputDirection.Normalize();
        }
        else 
        {
            outputDirection = Vector2.zero;
        }

        // Плавный поворот вектора движения (Lerp)
        _currentSteeringVelocity = Vector2.Lerp(
            _currentSteeringVelocity,
            outputDirection,
            Time.deltaTime * steeringLerpSpeed
        );

        // Финальный вызов метода движения из Entity
        Move(_currentSteeringVelocity);
    }

    // --- ЛОГИКА АТАКИ ---

    public override void Attack(Vector2 direction)
    {
        base.Attack(direction); // Поворачивает спрайт (в Entity)
        
        _attackDir = direction; // Запоминаем для расчетов урона/хитбоксов в наследниках
        
        if (_animator != null) 
            _animator.SetTrigger(animAttack); // Запуск анимации
    }

    protected void ResetCooldown()
    {
        _curCooldown = attackCooldown;
    }

    // --- СОСТОЯНИЕ ---

    public override void ResetEntity() 
    {
        base.ResetEntity();
        if (Brain != null) Brain.ResetBrain();
        
        _curCooldown = 0; // Готов к бою сразу после респавна
        _currentSteeringVelocity = Vector2.zero;
    }

    public override void TakeDamage(float amount) 
    {
        base.TakeDamage(amount);

        // Создание текста урона через пул
        var damageText = GamePools.FloatingDamages.Get(floatingDamagePrefab, transform.position, Quaternion.identity);
        damageText.gameObject.SetActive(true);
        damageText.Initialize(amount);
    }

    // Добавь этот метод в Enemy.cs
    protected override void Die()
    {
        base.Die(); // Запустит анимацию смерти, отключит коллайдер и поставит IsDead = true

        // Выключаем мозг!
        if (Brain != null)
        {
            Brain.ChangeState(new EnemyState_Dead());
            Brain.enabled = false; // Полностью глушим компонент, чтобы Update/FixedUpdate перестали тикать
        }
    }   
}