using InsideVentura.AI;
using UnityEngine;

// --- СОСТОЯНИЕ ПОГОНИ (АГРЕССИВНОЕ) ---
using InsideVentura.AI;
using UnityEngine;

public class Envy_ChaseState : EnemyState
{
    // НОВОЕ: Переменная-память. Используем nullable (Vector2?), 
    // чтобы понимать, есть у нас сохраненная точка или она пуста (null).
    private Vector2? _lastKnownPosition = null;

    public override void FixedUpdate()
    {
        var envy = (DevouringEnvy)EnemyInstance;
        if (envy.target == null) return;

        // 1. Игрок В ПОЛЕ ЗРЕНИЯ
        if (envy.HasLineOfSightToTarget())
        {
            // Обновляем память каждый кадр, пока видим игрока
            _lastKnownPosition = envy.target.position;

            float dist = Vector2.Distance(envy.transform.position, envy.target.position);

            if (dist <= envy.attackDistance)
            {
                if (envy.IsAttackReady)
                {
                    Brain.ChangeState(new Envy_AttackState());
                }
                else
                {
                    Vector2 moveDir = (envy.target.position - envy.transform.position).normalized;
                    envy.MoveWithSteering(moveDir);
                }
            }
            else
            {
                Vector2 moveDir = (envy.target.position - envy.transform.position).normalized;
                envy.MoveWithSteering(moveDir);
            }
        }
        // 2. Игрок ПРОПАЛ ИЗ ВИДУ (забежал за стену или слишком далеко)
        else
        {
            // Проверяем, есть ли у нас сохраненная точка
            if (_lastKnownPosition.HasValue)
            {
                float distToLastPos = Vector2.Distance(envy.transform.position, _lastKnownPosition.Value);

                // Если мы еще не добежали до точки (погрешность 0.5f, чтобы не дергаться)
                if (distToLastPos > 0.5f)
                {
                    // Бежим к последней известной позиции!
                    Vector2 moveDir = (_lastKnownPosition.Value - (Vector2)envy.transform.position).normalized;
                    envy.MoveWithSteering(moveDir);
                }
                else
                {
                    // Мы прибежали на место, где игрок был секунду назад, но его тут нет.
                    // Останавливаемся и стираем память.
                    envy.MoveWithSteering(Vector2.zero);
                    _lastKnownPosition = null; 
                    
                    // (Опционально) Тут можно перевести врага в состояние Idle,
                    // чтобы он постоял и "почесал репу", прежде чем пойти патрулировать.
                }
            }
            else
            {
                // Игрока не видим, и в памяти пусто — просто стоим
                envy.MoveWithSteering(Vector2.zero);
            }
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
