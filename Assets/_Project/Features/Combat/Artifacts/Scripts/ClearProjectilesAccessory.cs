using UnityEngine;

[CreateAssetMenu(fileName = "NewClearProjectilesAccessory", menuName = "Inside-Ventura/Artifacts/Accessory/ClearProjectiles")]
public class ClearProjectilesAccessory : Accessory {
    [SerializeField] private LayerMask projectileLayer;
    [SerializeField] private float radius = 3f;

    public override void ExecuteAbility(AccessoryInstance instance, Vector2 direction) {
        // Берем позицию игрока из инстанса
        var hits = Physics2D.OverlapCircleAll(instance.PlayerAccessor.Transform.position, radius, projectileLayer);
        foreach (var hit in hits) Destroy(hit.gameObject);
    }
}