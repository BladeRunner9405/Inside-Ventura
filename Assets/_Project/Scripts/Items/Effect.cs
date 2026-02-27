using UnityEngine;

public abstract class Effect : ScriptableObject
{
  public abstract void OnEquipThought(Artifact artifact, GameObject player);
  public abstract void OnUnequipThought(Artifact artifact, GameObject player);
}
