using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class DevouringEnvy : Enemy
{
    [Header("Envy Distances")]
    [Tooltip("Дистанция, на которой Зависть начинает кусать")]
    public float attackDistance = 1.1f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;

    [SerializeField]
    private AttackObject biteAttackPrefab;

    private float _curCooldown;
    public bool IsAttackReady => _curCooldown <= 0;

    private Vector2 _attackDir;

    protected override void Start()
    {
        base.Start();
        // Зависть всегда начинает с преследования
        Brain.Init(this, new Envy_ChaseState());
    }

    private void Update()
    {
        if (IsDead) return;

        if (_curCooldown > 0)
            _curCooldown -= Time.deltaTime;
    }

    public override void Attack(Vector2 direction)
    {
        _attackDir = direction;
        // Зависть кусает часто, анимация должна быть быстрой
        _view.PlayAttack(PerformBite, StartCooldown);
    }

    private void PerformBite()
    {
        if (target == null) return;

        // Создаем круговой хитбокс укуса
        var attackObj = GamePools.Hitboxes.Get(
            biteAttackPrefab,
            transform.position,
            Quaternion.identity
        );
        
        attackObj.gameObject.SetActive(true);

        // Передаем урон, слой игрока и направление укуса
        attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir);
    }

    private void StartCooldown()
    {
        _curCooldown = attackCooldown;
        // После укуса Зависть сразу возвращается в погоню, чтобы "прилипнуть" к игроку
        Brain.ChangeState(new Envy_ChaseState());
    }
}