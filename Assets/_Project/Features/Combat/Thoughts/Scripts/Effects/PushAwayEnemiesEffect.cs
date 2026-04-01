using UnityEngine;

[CreateAssetMenu(fileName = "PushAwayEnemiesEffect", menuName = "Inside-Ventura/Effects/PushAwayEnemiesEffect")]
public class PushAwayEnemiesEffect : Effect {
    [SerializeField] private float pushRadius = 3f;
    [SerializeField] private float pushDistance = 5f;
    [SerializeField] private float pushDuration = 0.5f;

    public override void OnEquipThought(ArtifactInstance artifactInstance) {
        if (artifactInstance is AccessoryInstance accessory) {
            accessory.OnAbilityUsed += OnAccessoryUsed;
        }
    }

    public override void OnUnequipThought(ArtifactInstance artifactInstance) {
        if (artifactInstance is AccessoryInstance accessory) {
            accessory.OnAbilityUsed -= OnAccessoryUsed;
        }
    }

    private void OnAccessoryUsed(AccessoryInstance accessory, Vector2 direction) {
        Vector2 playerPos = accessory.PlayerAccessor.Transform.position;
        var colliders = Physics2D.OverlapCircleAll(playerPos, pushRadius, LayerMask.GetMask("Enemy"));

        foreach (var col in colliders) {
            var enemy = col.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead) {
                var pushDirection = ((Vector2)enemy.transform.position - playerPos).normalized;
                if (pushDirection == Vector2.zero) pushDirection = Vector2.right;

                enemy.Dash(pushDirection, pushDistance, pushDuration);
            }
        }
    }
}