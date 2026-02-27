using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAccessory", menuName = "Inside-Ventura/Artifacts/Accessory/Dash")]
public class DashAccessory : Accessory {
  [SerializeField] private float distance = 3f;
  [SerializeField] private float speed = 1f;

  public override void UseAbility() {
  }
}
