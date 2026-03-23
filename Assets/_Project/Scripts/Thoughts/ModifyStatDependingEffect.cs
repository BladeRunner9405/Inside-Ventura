using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatDependingEffect", menuName = "Inside-Ventura/Effects/ModifyStatDependingEffect")]
public class ModifyStatDependingEffect : Effect {
  [SerializeField] private ModifiableStatName statName;
  [SerializeField] private StatOperationType operationType = StatOperationType.Add;

  [Header("Source")]
  [SerializeField] private ModifiableStatName sourceStatName;
  [SerializeField] private StatOperationType sourceOperationType = StatOperationType.Multiply;
  [SerializeField] private float sourceCoefficient = 1.2f;
  private ModifiableStat _coefficient;

  private StatModifier _modifier;

  public override void OnEquipThought(Artifact artifact) {
    var sourceStat = GetStat(sourceStatName, artifact);
    if (sourceStat == null) return;

    _coefficient = new ModifiableStat(sourceStat.Value);
    _coefficient.AddModifier(new StatModifier(sourceOperationType, sourceCoefficient));

    _modifier = new StatModifier(operationType, _coefficient.Value);

    var stat = GetStat(statName, artifact);

    var oldValue = stat?.Value; // чисто для дебага
    stat?.AddModifier(_modifier);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {stat?.Value}");
  }

  public override void OnUnequipThought(Artifact artifact) {
    var stat = GetStat(statName, artifact);

    var oldValue = stat?.Value; // чисто для дебага
    stat?.RemoveModifier(_modifier);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {stat?.Value}");
  }
}
