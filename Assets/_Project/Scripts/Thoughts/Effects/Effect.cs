using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  protected Stat GetStat(StatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat != null) return stat;

    var playerAccessor = artifact.PlayerAccessor;
    stat = playerAccessor.GetStat(statName);
    if (stat != null) return stat;

    stat = playerAccessor.Equipment.Accessory.GetStat(statName);
    if (stat != null) return stat;
    stat = playerAccessor.Equipment.Heart.GetStat(statName);
    if (stat != null) return stat;
    stat = playerAccessor.Equipment.Weapon.GetStat(statName);
    if (stat != null) return stat;

    stat = playerAccessor.Stats.GetStat(statName);
    if (stat != null) return stat;

    return null;
  }

  public abstract void OnEquipThought(Artifact artifact);
  public abstract void OnUnequipThought(Artifact artifact);
}
