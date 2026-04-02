using UnityEngine;

[CreateAssetMenu(
  fileName = "ModifyStatDependingEffect",
  menuName = "Inside-Ventura/Effects/ModifyStatDependingEffect"
)]
public class ModifyStatDependingEffect : Effect
{
  [SerializeField]
  private StatName statName;

  [SerializeField]
  private StatOperationType operationType = StatOperationType.Add;

  [Header("Source")]
  [SerializeField]
  private StatName sourceStatName;

  [SerializeField]
  private StatOperationType sourceOperationType = StatOperationType.Multiply;

  [SerializeField]
  private float sourceCoefficient = 1.2f;

  public override void OnEquipThought(ArtifactInstance artifactInstance)
  {
    var sourceStat = GetStat(sourceStatName, artifactInstance);
    if (sourceStat is not ModifiableStat modifiableSourceStat)
      return;

    // Вычисляем значение на основе источника
    float sourceValue = modifiableSourceStat.ModifiedValue;
    float finalValue =
      sourceOperationType == StatOperationType.Multiply
        ? sourceValue * sourceCoefficient
        : sourceValue + sourceCoefficient;

    var stat = GetStat(statName, artifactInstance);
    if (stat is ModifiableStat modifiableStat)
    {
      modifiableStat.AddModifier(new StatModifier(operationType, finalValue, this));
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
