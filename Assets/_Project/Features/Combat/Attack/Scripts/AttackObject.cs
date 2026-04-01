using System.Collections.Generic;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class AttackObject : InjectMonoBehaviour {
  [Header("Debug Info")] [SerializeField]
  protected float currentDamage;

  [SerializeField] protected LayerMask targetLayer;
  [SerializeField] protected float lifeTime;
  protected HashSet<Entity> hitEntities = new();

  protected float spawnTime;

  protected virtual void Update() {
    if (lifeTime > 0 && Time.time - spawnTime >= lifeTime) Despawn();
  }

  public virtual void Initialize(float damage, LayerMask layer, float timeToLive) {
    currentDamage = damage;
    targetLayer = layer;
    lifeTime = timeToLive;
    spawnTime = Time.time;

    hitEntities.Clear();
  }

  protected virtual void TryDealDamage(Collider2D col) {
    if (((1 << col.gameObject.layer) & targetLayer) == 0) return;

    if (col.TryGetComponent<Entity>(out var entity))
      if (!hitEntities.Contains(entity)) {
        entity.TakeDamage(currentDamage);
        hitEntities.Add(entity);
        OnEntityHit(entity);
      }
  }

  protected virtual void OnEntityHit(Entity entity) {
  }

  protected virtual void Despawn() {
    gameObject.SetActive(false); // свободный, если он просто выключен
  }
}
