public class HeartInstance : ArtifactInstance
{
  public readonly Heart HeartData;

  // Сюда мысли будут добавлять бонусы к здоровью или защите
  public ModifiableStat MaxHealthBonus { get; private set; }

  // Изменили конструктор: добавили PlayerAccessor и передали его в base()
  public HeartInstance(Heart baseData, PlayerAccessor playerAccessor)
    : base(baseData, playerAccessor)
  {
    HeartData = baseData;

    // Инициализируем стат базовым значением из SO
    MaxHealthBonus = new ModifiableStat(baseData.baseMaxHealthBonus);
  }

  public override Stat GetStat(StatName statName)
  {
    if (statName == StatName.MaxHealth)
      return MaxHealthBonus;
    return base.GetStat(statName);
  }
}
