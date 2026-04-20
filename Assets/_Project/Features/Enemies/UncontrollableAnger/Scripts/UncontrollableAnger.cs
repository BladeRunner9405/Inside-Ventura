using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class UncontrollableAnger : Enemy
{
    [Header("Distances")]
    public float playerDistance = 2.7f;
    public float retreatDistance = 2f;

    [SerializeField]
    private AttackObject waveAttackPrefab;

    protected override void Start()
    {
        base.Start();
        Brain.Init(this, new UA_ChaseState());
    }

    // Обрати внимание: в инспекторе у него теперь настраивается поле "Attack Cooldown" из Enemy
    // Update и Attack удалены!

    public override void OnAnimationEvent_Impact()
    {
        if (target == null || IsDead) return;

        var attackObj = GamePools.Hitboxes.Get(waveAttackPrefab, transform.position, Quaternion.identity);
        attackObj.gameObject.SetActive(true);
        attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir);
    }

    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        ResetCooldown();
        Brain.ChangeState(new UA_ChaseState());
    }
}