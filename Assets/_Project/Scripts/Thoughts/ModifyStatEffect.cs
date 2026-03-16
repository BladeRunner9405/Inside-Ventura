using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatEffect", menuName = "Inside-Ventura/Effects/ModifyStatEffect")]
public class ModifyStatEffect : Effect
{
  [SerializeField] private StatName statName;
  [SerializeField] private StatModifierType modifierType = StatModifierType.Add;
  [SerializeField] private float coefficient = 5f;

  private StatModifier _modifier;

  public override void OnEquipThought(Artifact artifact) {
    float oldValue = artifact.GetStat(statName).Value; // чисто для дебага

    _modifier = new StatModifier(coefficient, modifierType, this);
    artifact.GetStat(statName)?.AddModifier(_modifier);

    Debug.Log($"Изменен {statName}: был {oldValue}, стал {artifact.GetStat(statName)?.Value}");
  }

  public override void OnUnequipThought(Artifact artifact) {
    float oldValue = artifact.GetStat(statName).Value; // чисто для дебага

    artifact.GetStat(statName)?.RemoveModifier(_modifier);

    Debug.Log($"Изменен {statName}: был {oldValue}, стал {artifact.GetStat(statName)?.Value}");
  }
}
