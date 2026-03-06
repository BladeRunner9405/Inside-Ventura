using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAccessory", menuName = "Inside-Ventura/Artifacts/Accessory/Dash")]
public class DashAccessory : Accessory {
  [SerializeField] private readonly float distance = 3f;
  [SerializeField] private readonly float duration = 0.5f;

  protected override void UseAbility(GameObject playerObject, Vector2 direction) {
    var player = playerObject.GetComponent<Player>();
    player.Dash(direction, distance, duration);
  }
}
