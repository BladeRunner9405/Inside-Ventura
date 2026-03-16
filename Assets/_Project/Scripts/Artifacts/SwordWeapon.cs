using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordWeapon", menuName = "Inside-Ventura/Artifacts/Weapon/Sword")]
public class SwordWeapon : Weapon 
{
    [Header("Hitbox Prefab")]
    [SerializeField] private SectorAttackObject sectorAttackPrefab;

    [Header("Attack")] 
    [SerializeField] private float normalAngle = 90f;
    [SerializeField] private float normalRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Special Attack")] 
    [SerializeField] private float specialAngle = 30f;
    [SerializeField] private float specialRange = 5f;
    [SerializeField] private float specialDamageMultiplier = 2f;
    [SerializeField] private float lungeDistance = 15f;

    [Tooltip("Время жизни хитбокса в секундах. 0 = мгновенно")]
    [SerializeField] private float hitboxActiveTime = 0.1f; 

    protected override void Attack(Vector2 direction) 
    {
        UpdateCombo();

        var isSpecial = currentChainCount == ChainCount;

        var angle = isSpecial ? specialAngle : normalAngle;
        var range = isSpecial ? specialRange : normalRange;
        var damageAmount = isSpecial ? Damage * specialDamageMultiplier : Damage;

        var player = PlayerAccessor.Player;
        Vector2 playerPosition = player.transform.position;
        var dir = direction.normalized;

        var attackObj = (SectorAttackObject)GamePools.Hitboxes.Get(sectorAttackPrefab, playerPosition, Quaternion.identity);
        
        attackObj.gameObject.SetActive(true);

        attackObj.Initialize(
            damage: (int)damageAmount, 
            layer: enemyLayer, 
            timeToLive: hitboxActiveTime, 
            angle: angle, 
            radius: range, 
            direction: dir
        );

        if (isSpecial) 
        {
            player.Move(dir * lungeDistance);
        }
    }
}