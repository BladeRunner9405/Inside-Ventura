using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class EmotionalSetup : Enemy
{
    [Header("Setup Attack Settings")]
    public float orbSpeed = 0.7f;
    
    [SerializeField] 
    private AttackObject darkOrbPrefab;

    protected override void Start()
    {
        base.Start();
        Brain.Init(this, new ES_IdleState());
    }
    
    // Update и Attack удалены!

    public override void OnAnimationEvent_Impact()
    {
        if (IsDead) return;

        Vector2[] directions = {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1).normalized, new Vector2(1, -1).normalized,
            new Vector2(-1, 1).normalized, new Vector2(-1, -1).normalized
        };

        foreach (var dir in directions)
        {
            var orb = GamePools.Hitboxes.Get(darkOrbPrefab, transform.position, Quaternion.identity);
            orb.gameObject.SetActive(true);

            if (orb is ProjectileAttackObject projectile)
            {
                projectile.speed = orbSpeed;
            }

            orb.Initialize(damage, LayerMask.GetMask("Player"), dir);
        }
    }

    public override void OnAnimationEvent_End()
    {
        if (IsDead) return;

        ResetCooldown();
        Brain.ChangeState(new ES_IdleState());
    }
}