using UnityEngine;

public abstract class Weapon : Artifact {
  [SerializeField] protected float damage = 10f;
  [SerializeField] protected float attackSpeed = 0.2f;

  [SerializeField] protected int chainCount = 4;
  [SerializeField] protected float chainSpeedMultiplier = 1.5f;
  [SerializeField] protected float comboWindow = 0.5f;

  protected int currentChainCount;
  protected bool isInitialized;
  protected float lastAttackTime;
  protected float nextCooldown;

  public virtual void Initialize() {
    lastAttackTime = -attackSpeed;
    currentChainCount = 0;
    nextCooldown = attackSpeed;
    isInitialized = true;
  }

  public abstract void Attack(GameObject player, Vector2 direction);

  protected bool CanAttack() {
    if (!isInitialized) Initialize();
    return Time.time >= lastAttackTime + nextCooldown;
  }

  protected void UpdateCombo() {
    var tooLateForCombo = Time.time > lastAttackTime + comboWindow; // если опоздал на комбо

    if (tooLateForCombo) currentChainCount = 0;

    ++currentChainCount;

    // chainSpeedMultiplier - множитель, который используется на attackSpeed при совершении макс. комбо или при
    // преждевременном завершении предыдущего.
    // мб я чето не то сделал, надо будет спросить
    if (tooLateForCombo || currentChainCount == chainCount)
      nextCooldown = attackSpeed * chainSpeedMultiplier;
    else
      nextCooldown = attackSpeed;
  }
}
