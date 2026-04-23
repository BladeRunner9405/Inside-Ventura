using InsideVentura.AI;
using UnityEngine;

// --- СОСТОЯНИЕ ОЖИДАНИЯ ---
public class ES_IdleState : EnemyState
{
    public override void FixedUpdate()
    {
        var setup = (EmotionalSetup)EnemyInstance;
        
        // НОВОЕ: Если кулдаун прошел И мы ВИДИМ игрока — переходим в атаку
        if (setup.IsAttackReady && setup.HasLineOfSightToTarget())
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