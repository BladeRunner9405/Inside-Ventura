using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  public abstract void OnEquipThought(Artifact artifact, GameObject player);
  public abstract void OnUnequipThought(Artifact artifact, GameObject player);
}
