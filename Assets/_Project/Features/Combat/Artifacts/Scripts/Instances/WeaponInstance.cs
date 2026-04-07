using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponInstance : ArtifactInstance
{
  public readonly Weapon WeaponData; // Удобная ссылка на скастованную дату

  public ModifiableStat Damage { get; private set; }
  public ModifiableStat AttackSpeed { get; private set; }
  public ModifiableStat CritChance { get; private set; }
  public ModifiableStat ChainCount { get; private set; }
  public ModifiableStat ComboWindow { get; private set; }

  private float _lastAttackTime;
  private int _currentChainCount;
  private float _currentCooldown;

  public int CurrentChainCount { get; private set; }

  public void ResetChainCount() => CurrentChainCount = 0;

  public float GetDamageWithCritChance(float baseDamage)
  {
    var isCritical = Random.value <= CritChance.ModifiedValue;
    return isCritical ? baseDamage * WeaponData.baseCritMultiplier : baseDamage;
  }

  private void ExecuteAttack(Vector2 direction)
  {
    if (Time.time > _lastAttackTime + ComboWindow.ModifiedValue)
    {
      ResetChainCount();
    }
    CurrentChainCount++;

    float finalDamage = GetDamageWithCritChance(Damage.ModifiedValue);

    // Передаем логику атаки обратно в SO (SwordWeapon)
    WeaponData.ExecuteAttack(this, direction, finalDamage);

    _currentCooldown = AttackSpeed.ModifiedValue;
  }

  // Изменили конструктор: добавили PlayerAccessor и передали его в base()
  public WeaponInstance(Weapon baseData, PlayerAccessor playerAccessor)
    : base(baseData, playerAccessor)
  {
    WeaponData = baseData;

    // Инициализируем "живые" статы
    Damage = new ModifiableStat(baseData.baseDamage);
    AttackSpeed = new ModifiableStat(baseData.baseAttackSpeed);
    CritChance = new ModifiableStat(baseData.baseCritChance);
    ChainCount = new ModifiableStat(baseData.baseChainCount);
    ComboWindow = new ModifiableStat(baseData.baseComboWindow);

    _lastAttackTime = -AttackSpeed.ModifiedValue;
  }

  // Переопределяем метод для поиска статов по имени
  public override Stat GetStat(StatName statName)
  {
    if (statName == StatName.Damage)
      return Damage;
    if (statName == StatName.AttackSpeed)
      return AttackSpeed;
    if (statName == StatName.CritChance)
      return CritChance;
    if (statName == StatName.ChainCount)
      return ChainCount;
    return base.GetStat(statName);
  }

  public void TryAttack(Vector2 direction)
  {
    if (Time.time < _lastAttackTime + _currentCooldown)
      return;

    ExecuteAttack(direction);
    _lastAttackTime = Time.time;
  }
}
