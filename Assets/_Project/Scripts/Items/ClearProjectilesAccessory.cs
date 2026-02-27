using UnityEngine;

[CreateAssetMenu(fileName = "NewClearProjectilesAccessory",
  menuName = "Inside-Ventura/Artifacts/Accessory/ClearProjectiles")]
public class ClearProjectilesAccessory : Accessory {
  [SerializeField] private float radius = 3f;

  public override void UseAbility() {
  }
}
