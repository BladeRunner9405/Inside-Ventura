using System.Collections.Generic;
using UnityEngine;

public abstract class Artifact : ScriptableObject {
  [SerializeField] protected string artifactName;
  [SerializeField] protected int slotsCount;

  private List<Thought> equippedThoughts = new List<Thought>();

  protected void EquipThought(Thought thought, int slotIndex, IPlayer player) {
    if (slotIndex < 0 || slotIndex >= slotsCount) return;

    while (equippedThoughts.Count <= slotIndex)
      equippedThoughts.Add(null);

    Thought old = equippedThoughts[slotIndex];
    if (old != null)
      UnequipThought(slotIndex, player);

    equippedThoughts[slotIndex] = thought;
    thought.Equip(this, player);
  }

  protected void UnequipThought(int slotIndex, IPlayer player) {
    if (slotIndex < 0 || slotIndex >= equippedThoughts.Count) return;

    var thought = equippedThoughts[slotIndex];
    if (thought == null) return;

    thought.Unequip(this, player);

    equippedThoughts[slotIndex] = null;
  }
}
