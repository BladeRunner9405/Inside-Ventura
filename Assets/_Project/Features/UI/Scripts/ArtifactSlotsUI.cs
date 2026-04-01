using UnityEngine;

public class ArtifactSlotsUI : MonoBehaviour {
  [SerializeField] private ThoughtSlotUI[] slots;

  private ArtifactInstance _artifactInstance;

  public void Initialize(ArtifactInstance artifactInstance) {
    // Защита: если мы уже инициализированы этим артефактом, ничего не делаем
    if (_artifactInstance == artifactInstance) return; 

    // Защита: отписываемся от старого артефакта, если он был
    if (_artifactInstance != null) {
      _artifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
      _artifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;
    }

    _artifactInstance = artifactInstance;

    var count = Mathf.Min(slots.Length, artifactInstance.BaseData.SlotsCount);
    for (var i = 0; i < count; ++i) {
      var slot = slots[i];
      slot.SourceBag = null;
      slot.SourceArtifactInstance = artifactInstance;
      slot.ArtifactSlotIndex = i;
      slot.SetData(artifactInstance.EquippedThoughts[i]);
    }

    _artifactInstance.OnThoughtEquipped += HandleThoughtEquipped;
    _artifactInstance.OnThoughtUnequipped += HandleThoughtUnequipped;
  }

  private void OnDestroy() {
    if (_artifactInstance == null) return;

    _artifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
    _artifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;
  }

  private void HandleThoughtEquipped(int slotIndex, Thought thought) {
    if (slotIndex < slots.Length)
      slots[slotIndex].SetData(thought);
  }

  private void HandleThoughtUnequipped(int slotIndex) {
    if (slotIndex < slots.Length)
      slots[slotIndex].Clear();
  }
}