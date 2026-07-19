using UnityEngine;

public enum EffectType
{
  Positive,
  Negative,
  Neutral
}

public abstract class Effect : ScriptableObject
{
  [SerializeField] private string description;
  [SerializeField] private EffectType type;

  public string Description => description;
  public EffectType Type => type;

  // Изменили сигнатуры! Теперь принимают ArtifactInstance.
  public abstract void OnEquipThought(ArtifactInstance artifactInstance);
  public abstract void OnUnequipThought(ArtifactInstance artifactInstance);

  // Вспомогательный метод (если нужен)
  protected Stat GetStat(StatName statName, ArtifactInstance artifactInstance)
  {
    var stat = artifactInstance.GetStat(statName);
    if (stat != null) return stat;

    var playerAccessor = artifactInstance.PlayerAccessor;
    stat = playerAccessor.GetStat(statName);
    return stat;
  }

  public Effect GetCopy() {
    return Instantiate(this);
  }
}
