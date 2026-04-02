using UnityEngine;

public abstract class Effect : ScriptableObject
{
  // Изменили сигнатуры! Теперь принимают ArtifactInstance.
  public abstract void OnEquipThought(ArtifactInstance artifactInstance);
  public abstract void OnUnequipThought(ArtifactInstance artifactInstance);

  // Вспомогательный метод (если нужен)
  protected Stat GetStat(StatName statName, ArtifactInstance artifactInstance)
  {
    return artifactInstance.GetStat(statName);
  }
}
