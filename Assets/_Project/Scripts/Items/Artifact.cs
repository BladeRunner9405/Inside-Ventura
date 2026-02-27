using System.Collections.Generic;
using UnityEngine;

public abstract class Artifact : ScriptableObject {
  [SerializeField] public string artifactName;
  [SerializeField] protected int slotsCount;

  public int SlotsCount => slotsCount;

  private List<Thought> _equippedThoughts = new List<Thought>();

  public void EquipThought(Thought thought, int slotIndex, GameObject player) {
    if (slotIndex < 0 || slotIndex >= slotsCount) return;

    while (_equippedThoughts.Count <= slotIndex)
      _equippedThoughts.Add(null);

    Thought old = _equippedThoughts[slotIndex];
    if (old != null)
      UnequipThought(slotIndex, player);

    _equippedThoughts[slotIndex] = thought;
    thought.Equip(this, player);
  }

  public void UnequipThought(int slotIndex, GameObject player) {
    if (slotIndex < 0 || slotIndex >= _equippedThoughts.Count) return;

    var thought = _equippedThoughts[slotIndex];
    if (thought == null) return;

    thought.Unequip(this, player);

    _equippedThoughts[slotIndex] = null;
  }
}
