public class HeartInstance : ArtifactInstance
{
  public readonly Heart HeartData;

  // Сюда мысли будут добавлять бонусы к здоровью или защите
  // public ModifiableStat MaxHealthBonus { get; private set; }

  // Изменили конструктор: добавили PlayerAccessor и передали его в base()
  public HeartInstance(Heart baseData, PlayerAccessor playerAccessor)
    : base(baseData, playerAccessor)
  {
    HeartData = baseData;

    // Инициализируем стат базовым значением из SO
    // MaxHealthBonus = new ModifiableStat(baseData.baseMaxHealthBonus);

    var traits = HeartData.Traits;

    if (traits != null)
    {
      var traitsClone = new Effect[traits.Count];
      for (var i = 0; i < traits.Count; ++i)
      {
        traitsClone[i] = traits[i].GetCopy();
      }
      traits = traitsClone;

      foreach (var effect in traits)
        effect.OnEquipThought(this);
    }
  }

  /*public override Stat GetStat(StatName statName)
  {
    return base.GetStat(statName);
  }*/
}
