using System.Collections.Generic;
using UnityEngine;

public abstract class Artifact : ScriptableObject {
  [SerializeField] public string artifactName;
  [SerializeField] protected int slotsCount = 3;

  [SerializeField] private List<Thought> _equippedThoughts = new();

  public int SlotsCount => slotsCount;

  public virtual void Initialize(GameObject player) {
    RestoreThoughts(player);
  }

  public void EquipThought(Thought thought, int slotIndex, GameObject player) {
    if (slotIndex < 0 || slotIndex >= slotsCount) return;

    while (_equippedThoughts.Count <= slotIndex)
      _equippedThoughts.Add(null);

    var old = _equippedThoughts[slotIndex];
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

  protected void RestoreThoughts(GameObject player) {
    if (_equippedThoughts.Count > slotsCount) {
      Debug.Log($"Эээээ на {artifactName} экипировано больше мыслей, чем слотов. Ничего не экипирую.");
      _equippedThoughts = new List<Thought>();
      return;
    }

    for (var i = 0; i < _equippedThoughts.Count; ++i) {
      var thought = _equippedThoughts[i];
      if (thought)
        thought.Equip(this, player);
    }
  }
}
