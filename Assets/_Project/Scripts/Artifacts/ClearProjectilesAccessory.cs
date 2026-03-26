using UnityEngine;

[CreateAssetMenu(fileName = "NewClearProjectilesAccessory",
  menuName = "Inside-Ventura/Artifacts/Accessory/ClearProjectiles")]
public class ClearProjectilesAccessory : Accessory {
  [SerializeField] private LayerMask projectileLayer;
  [SerializeField] private float radius = 3f;

  protected override void UseAbility(Vector2 direction) {
    var hits = Physics2D.OverlapCircleAll(PlayerAccessor.Transform.position, radius, projectileLayer);
    foreach (var hit in hits) Destroy(hit.gameObject);
  }
}
