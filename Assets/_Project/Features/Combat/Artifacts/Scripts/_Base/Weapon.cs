using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "InsideVentura/Artifacts/Weapon")]
public abstract class Weapon : Artifact
{
  // Это БАЗОВЫЕ значения. Они не модифицируются.
  [Header("Base Stats")]
  [SerializeField]
  public float baseDamage = 10f;

  [SerializeField]
  public float baseAttackSpeed = 1f;

  [Header("Combo Stats")]
  [SerializeField]
  public float baseComboDamage = 25f;

  [SerializeField]
  public int baseChainCount = 3;

  [SerializeField]
  public float baseComboWindow = 0.5f;

  [SerializeField]
  public float baseChainSpeedMultiplier = 1.2f;

  [SerializeField]
  public float baseChainSpeedAddition = 0f;

  [Header("Critical Hit")]
  [SerializeField]
  [Range(0f, 1f)]
  public float baseCritChance = 0.1f;

  [SerializeField]
  public float baseCritMultiplier = 1.5f;

  // Префаб атаки или визуальный эффект (например, взмах мечом)
  // [SerializeField] public GameObject attackPrefab;

  public abstract void ExecuteAttack(WeaponInstance instance, Vector2 direction, float finalDamage);
}
