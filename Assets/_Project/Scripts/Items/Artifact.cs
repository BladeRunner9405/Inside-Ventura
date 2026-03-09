using CherryFramework.DependencyManager;
using System.Collections.Generic;
using UnityEngine;

public abstract class Artifact : ScriptableObject {
  [Inject] public PlayerAccessor PlayerAccessor;

  [SerializeField] public string artifactName;
  [SerializeField] protected int slotsCount = 3;

  [SerializeField] private List<Thought> equippedThoughts = new();

  public int SlotsCount => slotsCount;

  public virtual void Initialize() {
    RestoreThoughts();
  }

  public void EquipThought(Thought thought, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= slotsCount) return;

    while (equippedThoughts.Count <= slotIndex)
      equippedThoughts.Add(null);

    var old = equippedThoughts[slotIndex];
    if (old != null)
      UnequipThought(slotIndex);

    equippedThoughts[slotIndex] = thought;
    thought.Equip(this);
  }

  public void UnequipThought(int slotIndex) {
    if (slotIndex < 0 || slotIndex >= equippedThoughts.Count) return;

    var thought = equippedThoughts[slotIndex];
    if (thought == null) return;

    thought.Unequip(this);

    equippedThoughts[slotIndex] = null;
  }

  private void RestoreThoughts() {
    if (equippedThoughts.Count > slotsCount) {
      Debug.Log($"Эээээ на {artifactName} экипировано больше мыслей, чем слотов. Ничего не экипирую.");
      equippedThoughts = new List<Thought>();
      return;
    }

    for (var i = 0; i < equippedThoughts.Count; ++i) {
      var thought = equippedThoughts[i];
      if (thought)
        thought.Equip(this);
    }
  }
}
