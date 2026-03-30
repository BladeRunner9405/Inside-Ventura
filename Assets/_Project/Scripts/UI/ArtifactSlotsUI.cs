using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArtifactSlotsUI : MonoBehaviour
{
  [SerializeField] private ThoughtSlotUI[] slots; // загрузить слоты из инспектора
  private Artifact artifact;

  public void Initialize(Artifact artifact)
  {
    this.artifact = artifact;
    // Убедимся, что количество слотов совпадает с артефактом
    for (int i = 0; i < slots.Length && i < artifact.SlotsCount; ++i)
    {
      slots[i].SourceArtifact = artifact;
      slots[i].ArtifactSlotIndex = i;
      UpdateSlot(i);
    }

    artifact.OnThoughtEquipped += OnThoughtEquipped;
    artifact.OnThoughtUnequipped += OnThoughtUnequipped;
  }

  private void OnThoughtEquipped(int slotIndex, Thought thought)
  {
    if (slotIndex < slots.Length)
      slots[slotIndex].SetData(thought);
  }

  private void OnThoughtUnequipped(int slotIndex)
  {
    if (slotIndex < slots.Length)
      slots[slotIndex].Clear();
  }

  private void UpdateSlot(int slotIndex)
  {
    var thought = artifact.GetThoughtAtSlot(slotIndex);
    slots[slotIndex].SetData(thought);
  }

  private void OnDestroy()
  {
    if (artifact != null)
    {
      artifact.OnThoughtEquipped -= OnThoughtEquipped;
      artifact.OnThoughtUnequipped -= OnThoughtUnequipped;
    }
  }
}
