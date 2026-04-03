using UnityEngine;
using InsideVentura.AI;

// --- ПОГОНЯ ВПЛОТНУЮ ---
public class Envy_ChaseState : EnemyState
{
    public override void Update()
    {
        var envy = (Envy)EnemyInstance;
        if (envy.target == null) return;

        float dist = Vector2.Distance(envy.transform.position, envy.target.position);

        // Если добежали и готовы кусать
        if (dist <= envy.biteDistance && envy.IsAttackReady)
        {
            Brain.ChangeState(new Envy_AttackState());
            return;
        }

        // Бежим в лицо, используя Context Steering
        Vector2 dir = (envy.target.position - envy.transform.position).normalized;
        envy.MoveWithSteering(dir);
    }
}

// --- УКУС ---
public class Envy_AttackState : EnemyState
{
    public override void Enter()
    {
        // Останавливаемся для укуса
        EnemyInstance.MoveWithSteering(Vector2.zero);
        
        // Вместо Animator.SetTrigger вызываем логику последовательности
        ((Envy)EnemyInstance).StartBiteSequence();
    }
}