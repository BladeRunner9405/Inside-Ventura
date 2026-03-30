using System.Collections.Generic;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class Artifact : ScriptableObject {
  [SerializeField] public string artifactName;
  [SerializeField] protected int slotsCount = 3;

  [SerializeField] private List<Thought> equippedThoughts;
  [Inject] public PlayerAccessor PlayerAccessor;

  public int SlotsCount => slotsCount;

  public virtual Stat GetStat(StatName statName) {
    return null;
  }

  public virtual void Initialize() {
    if (equippedThoughts != null) {
      var cloned = new List<Thought>();
      for (var i = 0; i < equippedThoughts.Count; ++i) cloned.Add(Instantiate(equippedThoughts[i]));
      equippedThoughts = cloned;
    }

    EquipInitialThoughts();
  }

  public bool HasThought(Thought thought) {
    return equippedThoughts.Contains(thought);
  }

  public void EquipThought(Thought thought, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= slotsCount) return;

    if (!thought.HasRightType(this) || HasThought(thought)) return;

    while (equippedThoughts.Count <= slotIndex)
      equippedThoughts.Add(null);

    var old = equippedThoughts[slotIndex];
    if (old != null)
      UnequipThought(slotIndex);

    equippedThoughts[slotIndex] = thought;
    thought.OnEquip(this);
  }

  public void UnequipThought(int slotIndex) {
    if (slotIndex < 0 || slotIndex >= equippedThoughts.Count) return;

    var thought = equippedThoughts[slotIndex];
    if (thought == null) return;

    thought.OnUnequip(this);

    equippedThoughts[slotIndex] = null;
  }

  // Это метод для дебага
  private void EquipInitialThoughts() {
    if (equippedThoughts.Count > slotsCount) {
      Debug.Log($"Эээээ на {artifactName} экипировано больше мыслей, чем слотов. Ничего не экипирую.");
      equippedThoughts = new List<Thought>();
      return;
    }

    for (var i = 0; i < equippedThoughts.Count; ++i) {
      var thought = equippedThoughts[i];
      if (thought)
        thought.OnEquip(this);
    }
  }
}
