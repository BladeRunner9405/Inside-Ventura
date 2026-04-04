using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordWeapon", menuName = "Inside-Ventura/Artifacts/Weapon/Sword")]
public class SwordWeapon : Weapon
{
  [Header("Hitbox Prefab")]
  [SerializeField]
  private SectorAttackObject sectorAttackPrefab;

  [Header("Attack")]
  [SerializeField]
  private float normalAngle = 90f;

  [SerializeField]
  private float normalRange = 2f;

  [SerializeField]
  private LayerMask enemyLayer;

  [Header("Special Attack")]
  [SerializeField]
  private float specialAngle = 30f;

  [SerializeField]
  private float specialRange = 5f;

  [SerializeField]
  public float baseSpecialDamage = 25f; // Теперь это базовое число, а не ModifiableStat

  [SerializeField]
  private float lungeDistance = 3f;

  [SerializeField]
  private float lungeDuration = 0.1f; // Изменил 1f на 0.1f (рывок не должен длиться секунду)

  public override void ExecuteAttack(
    WeaponInstance instance,
    Vector2 direction,
    float normalFinalDamage
  )
  {
    var isSpecial =
      instance.CurrentChainCount == Mathf.RoundToInt(instance.ChainCount.ModifiedValue);

    var angle = isSpecial ? specialAngle : normalAngle;
    var range = isSpecial ? specialRange : normalRange;

    // Если удар специальный, считаем крит от базового спец-урона
    var finalDamage = isSpecial
      ? instance.GetDamageWithCritChance(baseSpecialDamage)
      : normalFinalDamage;

    Vector2 playerPosition = instance.PlayerAccessor.Transform.position;
    var dir = direction.normalized;

    var attackObj = (SectorAttackObject)
      GamePools.Hitboxes.Get(sectorAttackPrefab, playerPosition, Quaternion.identity);
    attackObj.gameObject.SetActive(true);

    attackObj.Initialize(finalDamage, enemyLayer, dir, angle, range);

    if (isSpecial)
    {
      instance.PlayerAccessor.Dash(dir, lungeDistance, lungeDuration);
      instance.ResetChainCount(); // Сбрасываем комбо после спец удара
    }
  }
}
