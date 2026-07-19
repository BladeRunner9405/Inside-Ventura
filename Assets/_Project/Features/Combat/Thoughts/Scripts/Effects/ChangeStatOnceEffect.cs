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

  private bool _wasEquipped;

  private void OnEnable() {
    _wasEquipped = false;
  }

  public override void OnEquipThought(ArtifactInstance artifactInstance) {
    if (_wasEquipped) return;

    var stat = GetStat(statName, artifactInstance);
    if (stat == null) return;

    var oldValue = stat.Value; // чисто для дебага
    stat.Change(operationType, coefficient);
    Debug.Log($"Изменен {statName}: был {oldValue}, стал {stat.Value}");

    _wasEquipped = true;
  }

  public override void OnUnequipThought(ArtifactInstance artifactInstance) {
    Debug.Log($"Снят модификатор с {statName}");
  }
}
