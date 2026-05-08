using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponInstance : ArtifactInstance
{
  public readonly Weapon WeaponData; // Удобная ссылка на скастованную дату

  public ModifiableStat Damage { get; private set; }
  public ModifiableStat AttackSpeed { get; private set; }
  public ModifiableStat ComboDamage { get; private set; }
  public ModifiableStat CritChance { get; private set; }
  public ModifiableStat ChainCount { get; private set; }
  public ModifiableStat ComboWindow { get; private set; }
  public ModifiableStat ChainSpeedMultiplier { get; private set; }
  public ModifiableStat ChainSpeedAddition { get; private set; }

  private float _lastAttackTime;
  private int _currentChainCount;
  private float _currentCooldown;

  public event Action<bool> OnAttack; // bool - комбо атака ли это

  public int CurrentChainCount { get; private set; }

  public void ResetChainCount() => CurrentChainCount = 0;

  public float GetDamageWithCritChance(float baseDamage)
  {
    var isCritical = Random.value <= CritChance.ModifiedValue;
    return isCritical ? baseDamage * WeaponData.baseCritMultiplier : baseDamage;
  }

  private void UpdateCombo()
  {
    var tooLateForCombo = Time.time > _lastAttackTime + ComboWindow.ModifiedValue;

    if (tooLateForCombo)
    {
      ResetChainCount();
    }

    ++CurrentChainCount;

    if (CurrentChainCount == Mathf.RoundToInt(ChainCount.ModifiedValue))
      _currentCooldown = AttackSpeed.ModifiedValue * ChainSpeedMultiplier.ModifiedValue + ChainSpeedAddition.ModifiedValue;
    else if (tooLateForCombo)
      _currentCooldown = AttackSpeed.ModifiedValue * ChainSpeedMultiplier.ModifiedValue;
    else
      _currentCooldown = AttackSpeed.ModifiedValue;
  }

  private void ExecuteAttack(Vector2 direction)
  {
    UpdateCombo();

    float finalDamage = GetDamageWithCritChance(Damage.ModifiedValue);
    var isCombo = CurrentChainCount == Mathf.RoundToInt(ChainCount.ModifiedValue);

    // Передаем логику атаки обратно в SO (SwordWeapon)
    WeaponData.ExecuteAttack(this, direction, finalDamage, isCombo);

    _currentCooldown = AttackSpeed.ModifiedValue;

    OnAttack?.Invoke(isCombo);
  }

  // Изменили конструктор: добавили PlayerAccessor и передали его в base()
  public WeaponInstance(Weapon baseData, PlayerAccessor playerAccessor)
    : base(baseData, playerAccessor)
  {
    WeaponData = baseData;

    // Инициализируем "живые" статы
    Damage = new ModifiableStat(baseData.baseDamage);
    AttackSpeed = new ModifiableStat(baseData.baseAttackSpeed);
    ComboDamage = new ModifiableStat(baseData.baseComboDamage);
    CritChance = new ModifiableStat(baseData.baseCritChance);
    ChainCount = new ModifiableStat(baseData.baseChainCount);
    ComboWindow = new ModifiableStat(baseData.baseComboWindow);
    ChainSpeedMultiplier = new ModifiableStat(baseData.baseChainSpeedMultiplier);
    ChainSpeedAddition = new ModifiableStat(baseData.baseChainSpeedAddition);

    _lastAttackTime = -AttackSpeed.ModifiedValue;
  }

  public void TryAttack(Vector2 direction)
  {
    if (Time.time < _lastAttackTime + _currentCooldown)
      return;

    ExecuteAttack(direction);
    _lastAttackTime = Time.time;
  }

  // Переопределяем метод для поиска статов по имени
  public override Stat GetStat(StatName statName)
  {
    switch (statName)
    {
      case StatName.Damage: return Damage;
      case StatName.AttackSpeed:   return AttackSpeed;
      case StatName.ComboDamage:   return ComboDamage;
      case StatName.CritChance:   return CritChance;
      case StatName.ChainCount:   return ChainCount;
      case StatName.ChainSpeedAddition:   return ChainSpeedAddition;
      default: return base.GetStat(statName);
    }
  }
}
