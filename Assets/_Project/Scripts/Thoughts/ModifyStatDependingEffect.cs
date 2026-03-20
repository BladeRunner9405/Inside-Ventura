using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatDependingEffect", menuName = "Inside-Ventura/Effects/ModifyStatDependingEffect")]
public class ModifyStatDependingEffect : Effect {
  [SerializeField] private StatName statName;
  [SerializeField] private StatModifierType modifierType = StatModifierType.Add;


  [SerializeField] private StatName sourceStatName;
  [SerializeField] private StatModifierType sourceModifierType = StatModifierType.Multiply;
  [SerializeField] private float sourceCoefficient = 5f;
  private ModifiableStat _coefficient;

  private StatModifier _modifier;

  public override void OnEquipThought(Artifact artifact) {
    var oldValue = artifact.GetStat(statName).Value; // чисто для дебага

    var sourceStatValue = artifact.GetStat(sourceStatName).Value;
    _coefficient = new ModifiableStat(sourceStatValue);
    _coefficient.AddModifier(new StatModifier(sourceModifierType, sourceCoefficient));

    _modifier = new StatModifier(modifierType, _coefficient.Value);
    artifact.GetStat(statName)?.AddModifier(_modifier);

    Debug.Log($"Изменен {statName}: был {oldValue}, стал {artifact.GetStat(statName)?.Value}");
  }

  public override void OnUnequipThought(Artifact artifact) {
    var oldValue = artifact.GetStat(statName).Value; // чисто для дебага

    artifact.GetStat(statName)?.RemoveModifier(_modifier);

    Debug.Log($"Изменен {statName}: был {oldValue}, стал {artifact.GetStat(statName)?.Value}");
  }
}
