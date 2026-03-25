using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatEffect", menuName = "Inside-Ventura/Effects/ModifyStatEffect")]
public class ModifyStatEffect : Effect {
  [SerializeField] private StatName statName;
  [SerializeField] private StatOperationType operationType = StatOperationType.Add;
  [SerializeField] private float coefficient = 5f;

  private StatModifier _modifier;

  public override void OnEquipThought(Artifact artifact) {
    _modifier = new StatModifier(operationType, coefficient);

    var stat = GetStat(statName, artifact);
    if (stat is not ModifiableStat modifiableStat) return;

    var oldValue = modifiableStat.ModifiedValue; // чисто для дебага
    modifiableStat.AddModifier(_modifier);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {modifiableStat.ModifiedValue}");
  }

  public override void OnUnequipThought(Artifact artifact) {
    var stat = GetStat(statName, artifact);
    if (stat is not ModifiableStat modifiableStat) return;

    var oldValue = modifiableStat.ModifiedValue; // чисто для дебага
    modifiableStat.RemoveModifier(_modifier);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {modifiableStat.ModifiedValue}");
  }
}
