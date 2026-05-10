using UnityEngine;

[CreateAssetMenu(fileName = "NewSwordWeapon", menuName = "InsideVentura/Artifacts/Weapon/Sword")]
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

  [Header("Combo Attack")]
  [SerializeField]
  private float comboAngle = 30f;

  [SerializeField]
  private float comboRange = 5f;

  [SerializeField]
  private float lungeDistance = 3f;

  [SerializeField]
  private float lungeDuration = 0.1f; // Изменил 1f на 0.1f (рывок не должен длиться секунду)

  public override void ExecuteAttack(
    WeaponInstance instance,
    Vector2 direction,
    float normalFinalDamage,
    bool isCombo
  )
  {
    var angle = isCombo ? comboAngle : normalAngle;
    var range = isCombo ? comboRange : normalRange;

    // Если удар специальный, считаем крит от базового спец-урона
    var finalDamage = isCombo
      ? instance.GetDamageWithCritChance(baseComboDamage)
      : normalFinalDamage;

    Vector2 playerPosition = instance.PlayerAccessor.Transform.position;
    var dir = direction.normalized;

    var attackObj = (SectorAttackObject)
      GamePools.Hitboxes.Get(sectorAttackPrefab, playerPosition, Quaternion.identity);
    attackObj.gameObject.SetActive(true);

    attackObj.Initialize(finalDamage, enemyLayer, dir, angle, range);

    if (isCombo)
    {
      instance.PlayerAccessor.Dash(dir, lungeDistance, lungeDuration, true);
      instance.ResetChainCount(); // Сбрасываем комбо после спец удара
    }
  }
}
