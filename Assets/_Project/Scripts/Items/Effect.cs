using UnityEngine;

public abstract class Effect : ScriptableObject
{
  public abstract void OnEquipThought(Thought thought, Artifact artifact, IPlayer player);
  public abstract void OnUnequipThought(Thought thought, Artifact artifact, IPlayer player);
}
