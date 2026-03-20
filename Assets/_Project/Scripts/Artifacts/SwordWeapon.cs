using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordWeapon", menuName = "Inside-Ventura/Artifacts/Weapon/Sword")]
public class SwordWeapon : Weapon {
  [Header("Hitbox Prefab")] [SerializeField]
  private SectorAttackObject sectorAttackPrefab;

  [Header("Attack")] [SerializeField] private float normalAngle = 90f;

  [SerializeField] private float normalRange = 2f;
  [SerializeField] private LayerMask enemyLayer;

  [Header("Special Attack")] [SerializeField]
  private float specialAngle = 30f;

  [SerializeField] private float specialRange = 5f;
  [SerializeField] private ModifiableStat specialDamage = new(4f);
  [SerializeField] private float lungeDistance = 3f;
  [SerializeField] private float lungeDuration = 1f;

  [Tooltip("Время жизни хитбокса в секундах. 0 = мгновенно")] [SerializeField]
  private float hitboxActiveTime = 0.1f;

  public float SpecialDamage => specialDamage.Value;

  public override ModifiableStat GetStat(StatName statName) {
    if (statName == StatName.SpecialDamage)
      return specialDamage;
    return base.GetStat(statName);
  }

  protected override void Attack(Vector2 direction) {
    UpdateCombo();

    var isSpecial = currentChainCount == ChainCount;

    var angle = isSpecial ? specialAngle : normalAngle;
    var range = isSpecial ? specialRange : normalRange;
    var baseDamage = isSpecial ? SpecialDamage : Damage;

    var finalDamage = GetDamageWithCritChance(baseDamage);

    var player = PlayerAccessor.Player;
    Vector2 playerPosition = player.transform.position;
    var dir = direction.normalized;

    var attackObj = (SectorAttackObject)GamePools.Hitboxes.Get(sectorAttackPrefab, playerPosition, Quaternion.identity);

    attackObj.gameObject.SetActive(true);

    attackObj.Initialize(
      finalDamage,
      enemyLayer,
      hitboxActiveTime,
      angle,
      range,
      dir
    );

    if (isSpecial) {
      player.Dash(dir, lungeDistance, lungeDuration);
      currentChainCount = 0;
    }
  }
}
