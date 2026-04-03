using UnityEngine;
using InsideVentura.AI;

// --- СОСТОЯНИЕ ПОГОНИ ---
public class UA_ChaseState : EnemyState
{
    public override void Update()
    {
        var ua = (UncontrollableAnger)EnemyInstance;
        float dist = Vector2.Distance(ua.transform.position, ua.target.position);

        // 1. Если ОЧЕНЬ близко — отходим
        if (dist < ua.retreatDistance) {
            Brain.ChangeState(new UA_RetreatState());
            return;
        }

        // 2. Если мы в "Идеальной зоне"
        if (dist >= ua.retreatDistance && dist <= ua.playerDistance) {
            if (ua.IsAttackReady) {
                Brain.ChangeState(new UA_AttackState());
            } else {
                // Стоим на месте, смотрим на игрока (ноль в steering)
                ua.MoveWithSteering(Vector2.zero); 
            }
            return;
        }

        // 3. Если далеко — идем к игроку
        if (dist > ua.playerDistance) {
            ua.MoveWithSteering((ua.target.position - ua.transform.position).normalized);
        }
    }
}

// --- СОСТОЯНИЕ ОТСТУПЛЕНИЯ ---
public class UA_RetreatState : EnemyState
{
    public override void Update()
    {
        var ua = (UncontrollableAnger)EnemyInstance;
        float dist = Vector2.Distance(ua.transform.position, ua.target.position);

        // Выходим из отступления чуть позже (2.2 вместо 2.0)
        if (dist >= ua.retreatDistance + 0.2f) 
        {
            Brain.ChangeState(new UA_ChaseState());
            return;
        }

        // Умное отступление (обходя препятствия за спиной)
        Vector2 desiredDir = (ua.transform.position - ua.target.position).normalized;
        ua.MoveWithSteering(desiredDir);
    }
}

// --- СОСТОЯНИЕ АТАКИ ---
public class UA_AttackState : EnemyState
{
    public override void Enter()
    {
        // Вместо Animator.SetTrigger:
        ((UncontrollableAnger)EnemyInstance).StartAttackSequence();
    }
}