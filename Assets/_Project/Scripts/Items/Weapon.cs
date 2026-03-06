using UnityEngine;

public abstract class Weapon : Artifact {
  [SerializeField] protected ModifiableStat damage = new(10f);
  [SerializeField] protected ModifiableStat attackSpeed = new(0.2f);
  [SerializeField] protected ModifiableStat chainCount = new(4f);
  [SerializeField] protected ModifiableStat chainSpeedMultiplier = new(1.5f);
  [SerializeField] protected float comboWindow = 0.5f;

  protected int currentChainCount;
  protected bool isInitialized;
  protected float lastAttackTime;
  protected float nextCooldown;

  public float Damage => damage.Value;
  public float AttackSpeed => attackSpeed.Value;
  public int ChainCount => Mathf.RoundToInt(chainCount.Value);
  public float ChainSpeedMultiplier => chainSpeedMultiplier.Value;

  public override void Initialize(GameObject player) {
    base.Initialize(player);
    currentChainCount = 0;
    lastAttackTime = -AttackSpeed;
    nextCooldown = AttackSpeed;
    isInitialized = true;
  }

  public abstract void Attack(GameObject player, Vector2 direction);

  protected bool CanAttack() {
    return Time.time >= lastAttackTime + nextCooldown;
  }

  protected void UpdateCombo() {
    var tooLateForCombo = Time.time > lastAttackTime + comboWindow; // если опоздал на комбо

    if (tooLateForCombo) currentChainCount = 0;

    ++currentChainCount;

    // chainSpeedMultiplier - множитель, который используется на attackSpeed при совершении макс. комбо или при
    // преждевременном завершении предыдущего.
    // мб я чето не то сделал, надо будет спросить
    if (tooLateForCombo || currentChainCount == ChainCount)
      nextCooldown = AttackSpeed * ChainSpeedMultiplier;
    else
      nextCooldown = AttackSpeed;
  }

  // к примеру
  public void AddDamageModifier(StatModifier modifier) {
    damage.AddModifier(modifier);
  }

  public void RemoveDamageModifier(StatModifier modifier) {
    damage.RemoveModifier(modifier);
  }
}
