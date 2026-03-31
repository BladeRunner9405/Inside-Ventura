using UnityEngine;

public class ArtifactSlotsUI : MonoBehaviour {
  [SerializeField] private ThoughtSlotUI[] slots;

  private Artifact _artifact;

  public void Initialize(Artifact artifact) {
    _artifact = artifact;

    var count = Mathf.Min(slots.Length, artifact.SlotsCount);
    for (var i = 0; i < count; ++i) {
      var slot = slots[i];
      slot.SourceBag = null;
      slot.SourceArtifact = artifact;
      slot.ArtifactSlotIndex = i;
      slot.SetData(artifact.GetThoughtAtSlot(i));
    }

    _artifact.OnThoughtEquipped += HandleThoughtEquipped;
    _artifact.OnThoughtUnequipped += HandleThoughtUnequipped;
  }

  private void OnDestroy() {
    if (_artifact == null) return;

    _artifact.OnThoughtEquipped -= HandleThoughtEquipped;
    _artifact.OnThoughtUnequipped -= HandleThoughtUnequipped;
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
