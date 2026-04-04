using System.Collections;
using UnityEngine;

public class SequenceAttackObject : AttackObject
{
    [Header("Spawner Settings")]
    [Tooltip("Префаб одиночного удара, который будем спавнить")]
    [SerializeField] private CircleAttackObject singleAttackPrefab;
    
    [Tooltip("Сколько всего кругов/ударов появится в волне")]
    [SerializeField] private int attackCount = 5;
    
    [Tooltip("Расстояние между центрами кругов")]
    [SerializeField] private float distanceBetweenAttacks = 1.5f;

    [Tooltip("Задержка перед появлением следующего круга (создает эффект волны)")]
    [SerializeField] private float delayBetweenSpawns = 0.1f;

    private Coroutine _sequenceCoroutine;

    private void OnDisable()
    {
        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
        }
    }

    public override void Initialize(float damage, LayerMask layer, Vector2 direction)
    {
        base.Initialize(damage, layer, direction);

        if (_sequenceCoroutine != null)
            StopCoroutine(_sequenceCoroutine);
            
        _sequenceCoroutine = StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        Vector2 currentSpawnPos = transform.position;

        for (int i = 0; i < attackCount; i++)
        {
            if (singleAttackPrefab != null)
            {
                // Спавним CircleAttackObject БЕЗ ПОВОРОТА (Quaternion.identity)
                // Если ты используешь пулы (CherryFramework), замени Instantiate на GamePools.Hitboxes.Get(...)
                var attackNode = Instantiate(singleAttackPrefab, currentSpawnPos, Quaternion.identity);
                
                attackNode.gameObject.SetActive(true);
                // Инициализируем созданный круг
                attackNode.Initialize(currentDamage, targetLayer, Direction);
            }

            // Сдвигаем позицию для следующего круга по вектору направления
            currentSpawnPos += Direction * distanceBetweenAttacks;

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        // Волна закончилась, спавнер больше не нужен
        Despawn();
    }
}