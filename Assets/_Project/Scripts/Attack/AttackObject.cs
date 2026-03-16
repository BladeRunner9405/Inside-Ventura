using System.Collections.Generic;
using UnityEngine;
using CherryFramework.DependencyManager;
public abstract class AttackObject : InjectMonoBehaviour 
{
    [Header("Debug Info")]
    [SerializeField] protected int currentDamage;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected float lifeTime;
    
    protected float spawnTime;
    protected HashSet<Entity> hitEntities = new HashSet<Entity>();

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
        if (((1 << col.gameObject.layer) & targetLayer) == 0) return;

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
        gameObject.SetActive(false); // свободный, если он просто выключен
    }
}