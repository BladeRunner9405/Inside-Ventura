using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAccessory", menuName = "Inside-Ventura/Artifacts/Accessory/Dash")]
public class DashAccessory : Accessory {
  [SerializeField] private float distance = 3f;
  [SerializeField] private float duration = 0.5f;

  protected override void UseAbility(Vector2 direction) {
    PlayerAccessor.Player.Dash(direction, distance, duration);
  }
}
