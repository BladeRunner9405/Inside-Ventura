using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAccessory", menuName = "Inside-Ventura/Artifacts/Accessory/Dash")]
public class DashAccessory : Accessory {
    [SerializeField] private float distance = 3f;
    [SerializeField] private float duration = 0.5f;

    public override void ExecuteAbility(AccessoryInstance instance, Vector2 direction) {
        // Делаем рывок через инстанс
        instance.PlayerAccessor.Dash(direction, distance, duration);
    }
}