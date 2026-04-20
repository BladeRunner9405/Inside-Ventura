using InsideVentura.AI;
using UnityEngine;

// --- СОСТОЯНИЕ ПОГОНИ (АГРЕССИВНОЕ) ---
public class Envy_ChaseState : EnemyState
{
    public override void FixedUpdate()
    {
        var envy = (DevouringEnvy)EnemyInstance;
        if (envy.target == null) return;

        float dist = Vector2.Distance(envy.transform.position, envy.target.position);

        // 1. Если добежали вплотную и готовы кусать
        if (dist <= envy.attackDistance)
        {
            if (envy.IsAttackReady)
            {
                Brain.ChangeState(new Envy_AttackState());
            }
            else
            {
                // Если перезарядка — продолжаем "липнуть", пытаясь быть максимально близко
                Vector2 moveDir = (envy.target.position - envy.transform.position).normalized;
                envy.MoveWithSteering(moveDir);
            }
        }
        else
        {
            // 2. Бежим к игроку, обходя препятствия
            Vector2 moveDir = (envy.target.position - envy.transform.position).normalized;
            envy.MoveWithSteering(moveDir);
        }
    }
}

// --- СОСТОЯНИЕ АТАКИ (УКУС) ---
public class Envy_AttackState : EnemyState
{
    public override void Enter()
    {
        EnemyInstance.MoveWithSteering(Vector2.zero);

        Vector2 dir = (EnemyInstance.target.position - EnemyInstance.transform.position).normalized;

        EnemyInstance.Attack(dir);
    }
}
