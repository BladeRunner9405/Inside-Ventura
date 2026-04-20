using InsideVentura.AI;
using UnityEngine;

// --- СОСТОЯНИЕ ОЖИДАНИЯ ---
public class ES_IdleState : EnemyState
{
    public override void FixedUpdate()
    {
        var setup = (EmotionalSetup)EnemyInstance;
        
        // Как только кулдаун прошел — переходим в атаку
        if (setup.IsAttackReady)
        {
            Brain.ChangeState(new ES_AttackState());
        }
    }
}

// --- СОСТОЯНИЕ АТАК ---
public class ES_AttackState : EnemyState
{
    public override void Enter()
    {
        // Вызываем атаку (направление неважно, так как она стреляет во все стороны)
        EnemyInstance.Attack(Vector2.zero);
    }
}