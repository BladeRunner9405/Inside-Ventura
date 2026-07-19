using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class DevouringEnvy : Enemy
{
    [Header("Envy Distances")]
    [Tooltip("Дистанция, на которой Зависть начинает кусать")]
    public float attackDistance = 1.1f;

    [SerializeField]
    private AttackObject biteAttackPrefab;

    protected override void Start()
    {
        base.Start();
        Brain.Init(this, new Envy_ChaseState());
    }

    // Update() и Attack() больше не нужны! Они работают в базовом классе.

    public override void OnAnimationEvent_Impact()
    {
        if (target == null || IsDead) return;

        var attackObj = GamePools.Hitboxes.Get(biteAttackPrefab, transform.position, Quaternion.identity);
        attackObj.gameObject.SetActive(true);
        attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir); // _attackDir берем из Enemy
    }

    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        ResetCooldown(); // Метод из базового класса Enemy
        Brain.ChangeState(new Envy_ChaseState());
    }
}