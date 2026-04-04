using UnityEngine;

[CreateAssetMenu(
  fileName = "ChangeStatOnceEffect",
  menuName = "InsideVentura/Effects/ChangeStatOnceEffect"
)]
public class ChangeStatOnceEffect : Effect
{
  [SerializeField]
  private StatName statName;

  [SerializeField]
  private StatOperationType operationType = StatOperationType.Add;

  [SerializeField]
  private float coefficient = 5f;

  public override void OnEquipThought(ArtifactInstance artifactInstance)
  {
    var stat = GetStat(statName, artifactInstance);

    if (stat is ModifiableStat modifiableStat)
    {
      // Привязываем this как источник
      modifiableStat.AddModifier(new StatModifier(operationType, coefficient, this));
    }
    else if (stat != null)
    {
      // Для обычных статов, у которых остался метод Change (например, деньги)
      stat.Change(operationType, coefficient);
    }
  }

  public override void OnUnequipThought(ArtifactInstance artifactInstance)
  {
    var stat = GetStat(statName, artifactInstance);

    if (stat is ModifiableStat modifiableStat)
    {
      modifiableStat.RemoveModifiersFromSource(this);
    }
  }
}
