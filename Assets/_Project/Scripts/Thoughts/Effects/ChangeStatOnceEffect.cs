using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatOnceEffect", menuName = "Inside-Ventura/Effects/ChangeStatOnceEffect")]
public class ChangeStatOnceEffect : Effect {
  [SerializeField] private DynamicStatName statName;
  [SerializeField] private StatOperationType operationType = StatOperationType.Add;
  [SerializeField] private float coefficient = 5f;

  private bool _wasEquipped;

  private void OnEnable() {
    _wasEquipped = false;
  }

  public override void OnEquipThought(Artifact artifact) {
    if (_wasEquipped) return;

    var stat = GetStat(statName, artifact);

    var oldValue = stat?.BaseValue; // чисто для дебага
    stat?.Change(operationType, coefficient);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {stat?.BaseValue}");

    _wasEquipped = true;
  }

  public override void OnUnequipThought(Artifact artifact) {
  }
}
