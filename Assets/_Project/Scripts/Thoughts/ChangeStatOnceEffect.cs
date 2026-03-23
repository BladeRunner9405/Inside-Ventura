using UnityEngine;

public class ChangeStatOnceEffect : Effect {
  [SerializeField] private StatName statName;
  [SerializeField] private StatModifierType modifierType = StatModifierType.Add;
  [SerializeField] private float coefficient = 5f;

  private StatModifier _modifier;

  public override void OnEquipThought(Artifact artifact) {
    _modifier = new StatModifier(modifierType, coefficient);

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
