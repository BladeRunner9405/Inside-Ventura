using InsideVentura.AI;
using UnityEngine;

// --- СОСТОЯНИЕ ПОГОНИ ---
public class UA_ChaseState : EnemyState
{
    // НОВОЕ: Переменная-память для хранения последней позиции игрока
    private Vector2? _lastKnownPosition = null;

    public override void FixedUpdate()
    {
        var ua = (UncontrollableAnger)EnemyInstance;
        if (ua.target == null) return;

        // 1. ИГРОК В ЗОНЕ ВИДИМОСТИ
        if (ua.HasLineOfSightToTarget())
        {
            // Обновляем память
            _lastKnownPosition = ua.target.position;

            float dist = Vector2.Distance(ua.transform.position, ua.target.position);

            // 1.1 Если ОЧЕНЬ близко — отходим
            if (dist < ua.retreatDistance)
            {
                Brain.ChangeState(new UA_RetreatState());
                return;
            }

            // 1.2 Если мы в "Идеальной зоне" (для атаки)
            if (dist >= ua.retreatDistance && dist <= ua.playerDistance)
            {
                if (ua.IsAttackReady)
                {
                    Brain.ChangeState(new UA_AttackState());
                }
                else
                {
                    // Стоим на месте, смотрим на игрока
                    ua.MoveWithSteering(Vector2.zero);
                }
                return;
            }

            // 1.3 Если далеко — идем к игроку
            if (dist > ua.playerDistance)
            {
                ua.MoveWithSteering((ua.target.position - ua.transform.position).normalized);
            }
        }
        // 2. ИГРОК ПРОПАЛ ИЗ ВИДУ (за стеной)
        else
        {
            // Проверяем, есть ли у нас сохраненная точка
            if (_lastKnownPosition.HasValue)
            {
                float distToLastPos = Vector2.Distance(ua.transform.position, _lastKnownPosition.Value);

                // Если еще не добежали (погрешность 0.5f)
                if (distToLastPos > 0.5f)
                {
                    Vector2 moveDir = (_lastKnownPosition.Value - (Vector2)ua.transform.position).normalized;
                    ua.MoveWithSteering(moveDir);
                }
                else
                {
                    // Добежали до угла, но игрока нет - останавливаемся
                    ua.MoveWithSteering(Vector2.zero);
                    _lastKnownPosition = null;
                }
            }
            else
            {
                // Нет памяти и нет видимости - просто стоим
                ua.MoveWithSteering(Vector2.zero);
            }
        }
    }
}

// --- СОСТОЯНИЕ ОТСТУПЛЕНИЯ ---
public class UA_RetreatState : EnemyState
{
    public override void FixedUpdate()
    {
        var ua = (UncontrollableAnger)EnemyInstance;
        
        // (Опционально) Если пока мы отступали, игрок спрятался за стену - 
        // можно прекратить отступление и перейти в погоню (поиск)
        if (!ua.HasLineOfSightToTarget())
        {
             Brain.ChangeState(new UA_ChaseState());
             return;
        }

        float dist = Vector2.Distance(ua.transform.position, ua.target.position);

        // Выходим из отступления чуть позже (2.2 вместо 2.0)
        if (dist >= ua.retreatDistance + 0.2f)
        {
            Brain.ChangeState(new UA_ChaseState());
            return;
        }

        // Умное отступление (обходя препятствия за спиной)
        // Вектор ОТ игрока
        Vector2 desiredDir = (ua.transform.position - ua.target.position).normalized;
        ua.MoveWithSteering(desiredDir);
    }
}

// --- СОСТОЯНИЕ АТАКИ ---
public class UA_AttackState : EnemyState
{
    public override void Enter()
    {
        // ВАЖНО: Жестко бьем по тормозам перед кастом волны!
        EnemyInstance.MoveWithSteering(Vector2.zero);

        // Вычисляем направление к игроку
        Vector2 dir = (EnemyInstance.target.position - EnemyInstance.transform.position).normalized;

        // Вызываем базовый метод!
        EnemyInstance.Attack(dir);
    }
}