using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  protected ModifiableStat GetStat(ModifiableStatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat != null) return stat;

    var player = artifact.PlayerAccessor.Player;
    stat = player.GetStat(statName);
    if (stat != null) return stat;

    stat = player.Equipment.Accessory.GetStat(statName);
    if (stat != null) return stat;
    stat = player.Equipment.Heart.GetStat(statName);
    if (stat != null) return stat;
    stat = player.Equipment.Weapon.GetStat(statName);
    if (stat != null) return stat;

    stat = player.Stats.GetStat(statName);
    if (stat != null) return stat;

    return null;
  }

  protected DynamicStat GetStat(DynamicStatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat != null) return stat;

    var player = artifact.PlayerAccessor.Player;
    stat = player.GetStat(statName);
    if (stat != null) return stat;

    stat = player.Equipment.Accessory.GetStat(statName);
    if (stat != null) return stat;
    stat = player.Equipment.Heart.GetStat(statName);
    if (stat != null) return stat;
    stat = player.Equipment.Weapon.GetStat(statName);
    if (stat != null) return stat;

    stat = player.Stats.GetStat(statName);
    if (stat != null) return stat;

    return null;
  }

  public abstract void OnEquipThought(Artifact artifact);
  public abstract void OnUnequipThought(Artifact artifact);
}
