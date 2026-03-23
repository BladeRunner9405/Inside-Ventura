using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  protected ModifiableStat GetStat(ModifiableStatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat == null) {
      var player = artifact.PlayerAccessor.Player;
      stat = player.GetStat(statName);
    }

    return stat;
  }

  protected DynamicStat GetStat(DynamicStatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat == null) {
      var player = artifact.PlayerAccessor.Player;
      stat = player.GetStat(statName);
    }

    return stat;
  }

  public abstract void OnEquipThought(Artifact artifact);
  public abstract void OnUnequipThought(Artifact artifact);
}
