using System.Collections.Generic;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class AttackObject : InjectMonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] protected AttackAnimator animator; // ПЕРЕНЕСЛИ СЮДА!

    [Header("Debug Info")]
    [SerializeField] protected float currentDamage;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected float lifeTime;
    
    public Vector2 Direction { get; protected set; }

    protected HashSet<Entity> hitEntities = new();
    protected float spawnTime;

    protected virtual void Update()
    {
        if (lifeTime > 0 && Time.time - spawnTime >= lifeTime)
            Despawn();
    }

    public virtual void Initialize(float damage, LayerMask layer, Vector2 direction)
    {
        currentDamage = damage;
        targetLayer = layer;
        
        Direction = direction.normalized;
        spawnTime = Time.time;
        hitEntities.Clear();

        // БАЗА САМА РЕШАЕТ, КАК ЗАПУСКАТЬ АТАКУ
        if (animator != null)
        {
            // PerformAttack вызовется на нужном кадре анимации
            animator.Play(PerformAttack, Despawn, Direction);
        }
        else
        {
            // Если аниматора нет (например, у спавнера), просто выполняем логику
            PerformAttack();
        }
    }

    // НОВЫЙ МЕТОД: Дочерние классы пишут геометрию атаки здесь
    protected virtual void PerformAttack() 
    { 
        // По умолчанию ничего не делает.
    }

    protected virtual void TryDealDamage(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & targetLayer) == 0)
            return;

        if (col.TryGetComponent<Entity>(out var entity))
        {
            if (!hitEntities.Contains(entity))
            {
                entity.TakeDamage(currentDamage);
                hitEntities.Add(entity);
                OnEntityHit(entity);
            }
        }
    }

    protected virtual void OnEntityHit(Entity entity) { }

    protected virtual void Despawn()
    {
        gameObject.SetActive(false);
    }
}