using UnityEngine;

public abstract class Effect : ScriptableObject {
  [SerializeField] private string description;

  public abstract void OnEquipThought(Artifact artifact);
  public abstract void OnUnequipThought(Artifact artifact);
}
