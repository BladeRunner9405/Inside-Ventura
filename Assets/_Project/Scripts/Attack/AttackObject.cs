using System.Collections.Generic;
using UnityEngine;

public abstract class AttackObject : MonoBehaviour 
{
    [Header("Debug Info")]
    [SerializeField] protected int currentDamage;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected float lifeTime;
    
    protected float spawnTime;

    protected HashSet<Entity> hitEntities = new HashSet<Entity>();  // те, кого уже продамажили, чтобы не дамажить их постоянно


    public virtual void Initialize(int damage, LayerMask layer, float timeToLive) 
    {
        currentDamage = damage;
        targetLayer = layer;
        lifeTime = timeToLive;
        spawnTime = Time.time;
        
        hitEntities.Clear();
    }

    protected virtual void Update() 
    {
        if (lifeTime > 0 && Time.time - spawnTime >= lifeTime) 
        {
            Despawn();
        }
    }

    protected virtual void TryDealDamage(Collider2D col) 
    {
        if (((1 << col.gameObject.layer) & targetLayer) == 0) return; // сверяем маски

        if (col.TryGetComponent<Entity>(out var entity)) 
        {
            if (!hitEntities.Contains(entity)) // не били ли мы его за текущую атаку
            {
                entity.TakeDamage(currentDamage);
                hitEntities.Add(entity);
                Debug.Log($"I hit {entity.name}");
                OnEntityHit(entity);
            }
        }
    }

    protected virtual void OnEntityHit(Entity entity) 
    {
        // ...
    }

    protected virtual void Despawn() 
    {
        SimplePool.Instance.Despawn(gameObject); 
    }
}