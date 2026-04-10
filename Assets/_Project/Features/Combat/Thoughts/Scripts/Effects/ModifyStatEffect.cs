using UnityEngine;

[CreateAssetMenu(menuName = "InsideVentura/Effects/Modify Stat")]
public class ModifyStatEffect : Effect
{
  [SerializeField]
  private StatName statName;

  [SerializeField]
  private StatOperationType operationType;

  [SerializeField]
  private float value;

  public override void OnEquipThought(ArtifactInstance artifactInstance)
  {
    var stat = GetStat(statName, artifactInstance);
    if (stat == null) {
      Debug.LogWarning($"Стата {statName} не найдена. Её никто не возвращает");
    }

    if (stat is ModifiableStat modifiableStat)
    {
      // Передаем `this` как источник модификатора
      var modifier = new StatModifier(operationType, value, this);
      modifiableStat.AddModifier(modifier);
      Debug.Log($"Добавлен модификатор на {statName}: {value}");
    }
  }

  public override void OnUnequipThought(ArtifactInstance artifactInstance)
  {
    var stat = GetStat(statName, artifactInstance);
    if (stat is ModifiableStat modifiableStat)
    {
      // Удаляем все модификаторы, которые были добавлены именно ЭТИМ эффектом
      modifiableStat.RemoveModifiersFromSource(this);
      Debug.Log($"Снят модификатор с {statName}");
    }
  }
}
