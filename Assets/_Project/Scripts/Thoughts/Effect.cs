using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  protected ModifiableStat GetStat(StatName statName, Artifact artifact) {
    var stat = artifact.GetStat(statName);
    if (stat == null) {
      Debug.Log(artifact.ToString());
      Debug.Log(artifact.PlayerAccessor?.ToString());
      Debug.Log(artifact.PlayerAccessor?.Player?.ToString());
      var player = artifact.PlayerAccessor.Player;
      stat = player.GetStat(statName);
    }

    return stat;
  }

  public abstract void OnEquipThought(Artifact artifact);
  public abstract void OnUnequipThought(Artifact artifact);
}
