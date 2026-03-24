using UnityEngine;

public abstract class Weapon : Artifact {
  [SerializeField] protected ModifiableStat damage = new(3f);
  [SerializeField] protected ModifiableStat attackSpeed = new(0.2f);
  [SerializeField] protected ModifiableStat chainCount = new(4f);
  [SerializeField] protected ModifiableStat chainSpeedMultiplier = new(1.2f);
  [SerializeField] protected ModifiableStat chainSpeedAddition = new();
  [SerializeField] protected ModifiableStat comboWindow = new(0.5f);

  [Header("Critical Hit")] [SerializeField]
  protected ModifiableStat critChance = new(0.05f);

  [SerializeField] protected ModifiableStat critMultiplier = new(1.2f);
  protected float cooldown;

  protected int currentChainCount;
  protected float lastAttackTime;

  public float Damage => damage.ModifiedValue;
  public float AttackSpeed => attackSpeed.ModifiedValue;
  public int ChainCount => Mathf.RoundToInt(chainCount.ModifiedValue);
  public float ChainSpeedMultiplier => chainSpeedMultiplier.ModifiedValue;
  public float ChainSpeedAddition => chainSpeedAddition.ModifiedValue;
  public float ComboWindow => comboWindow.ModifiedValue;
  public float CritChance => Mathf.Min(1f, critChance.ModifiedValue);
  public float CritMultiplier => critMultiplier.ModifiedValue;

  public override Stat GetStat(StatName statName) {
    if (statName == StatName.Damage)
      return damage;
    if (statName == StatName.ChainCount)
      return chainCount;
    if (statName == StatName.ChainSpeedAddition)
      return chainSpeedAddition;
    return base.GetStat(statName);
  }

  public override void Initialize() {
    base.Initialize();
    currentChainCount = 0;
    lastAttackTime = -AttackSpeed;
    cooldown = AttackSpeed;
  }

  protected bool CanAttack() {
    return Time.time >= lastAttackTime + cooldown;
  }

  public void TryAttack(Vector2 direction) {
    if (!CanAttack()) return;

    Attack(direction);
    lastAttackTime = Time.time; // фиксируем время удара
  }

  protected void UpdateCombo() {
    var tooLateForCombo = Time.time > lastAttackTime + ComboWindow; // если опоздал на комбо

    if (tooLateForCombo) currentChainCount = 0;

    ++currentChainCount;

    if (currentChainCount == ChainCount)
      cooldown = AttackSpeed * ChainSpeedMultiplier + ChainSpeedAddition;
    else if (tooLateForCombo)
      cooldown = AttackSpeed * ChainSpeedMultiplier;
    else
      cooldown = AttackSpeed;
  }

  protected float GetDamageWithCritChance(float baseDamage) {
    var isCritical = Random.value <= CritChance;
    var finalDamage = isCritical ? baseDamage * CritMultiplier : baseDamage;

    if (isCritical)
      Debug.Log($"Критический урон! {baseDamage} -> {finalDamage}");

    return finalDamage;
  }

  protected abstract void Attack(Vector2 direction);
}
