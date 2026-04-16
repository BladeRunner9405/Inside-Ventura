using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewItemChances", menuName = "InsideVentura/Chances/ItemChances")]
public class ItemChancesData : ScriptableObject
{
  [Header("Базовое количество")]
  public int baseMin;
  public int baseMax;

  [Header("Уровень 1 - дополнительно")]
  [Range(0f, 1f)] public float chanceMore;
  public int moreMin;
  public int moreMax;

  [Header("Уровень 2 - дополнительно (приоритет выше Уровня 1)")]
  [Range(0f, 1f)] public float chanceMuchMore;
  public int muchMoreMin;
  public int muchMoreMax;

  [Header("Особые варианты предмета (заменяют стандартный с указанным шансом)")]
  public SpecialItemVariant[] specialVariants;

  public int Roll()
  {
    int count = Random.Range(baseMin, baseMax + 1);

    float roll = Random.value;

    if (chanceMuchMore > 0f && roll < chanceMuchMore)
      count += Random.Range(muchMoreMin, muchMoreMax + 1);
    else if (chanceMore > 0f && roll < chanceMore)
      count += Random.Range(moreMin, moreMax + 1);

    // Debug.Log($"[{name}] {count}");

    return count;
  }

  public GameObject PickPrefab(GameObject defaultPrefab)
  {
    if (specialVariants == null || specialVariants.Length == 0)
      return defaultPrefab;

    float roll = Random.value;
    foreach (var variant in specialVariants)
    {
      if (roll < variant.chance)
        return variant.prefab;
    }

    return defaultPrefab;
  }
}
