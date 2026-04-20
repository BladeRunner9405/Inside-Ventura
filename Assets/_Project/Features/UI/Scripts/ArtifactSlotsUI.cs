using CherryFramework.DependencyManager;
using UnityEngine;

public class ArtifactSlotsUI : InjectMonoBehaviour
{
  [SerializeField]
  private ThoughtSlotUI[] slots;
  [SerializeField]
  private ArtifactTooltip artifactTooltip;

  [Inject]
  private ThoughtsCompatibilityManager _thoughtsCompatibilityManager;

  private bool _pinned = false;

  public ArtifactInstance ArtifactInstance { get; private set; }

  public void Initialize(ArtifactInstance artifactInstance)
  {
    // Защита: если мы уже инициализированы этим артефактом, ничего не делаем
    if (ArtifactInstance == artifactInstance)
      return;

    // Защита: отписываемся от старого артефакта, если он был
    if (ArtifactInstance != null)
    {
      ArtifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
      ArtifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;
    }

    ArtifactInstance = artifactInstance;

    if (artifactTooltip != null)
      artifactTooltip.Initialize(artifactInstance);

    var count = Mathf.Min(slots.Length, artifactInstance.BaseData.SlotsCount);
    for (var i = 0; i < count; ++i)
    {
      var slot = slots[i];
      slot.SourceBag = null;
      slot.SourceArtifactInstance = artifactInstance;
      slot.ArtifactSlotIndex = i;
      slot.SetData(artifactInstance.EquippedThoughts[i]);
    }

    ArtifactInstance.OnThoughtEquipped += HandleThoughtEquipped;
    ArtifactInstance.OnThoughtUnequipped += HandleThoughtUnequipped;
  }

  private void OnDestroy()
  {
    if (ArtifactInstance == null)
      return;

    ArtifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
    ArtifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;
  }

  public void ToggleActivity() => _pinned = !_pinned;

  public void SetAsActive() {
    if (_thoughtsCompatibilityManager.ActiveArtifact == null)
      _thoughtsCompatibilityManager.SetActiveArtifact(ArtifactInstance);
  }

  public void UnsetAsActive()
  {
    if (!_pinned && _thoughtsCompatibilityManager.ActiveArtifact == ArtifactInstance)
      _thoughtsCompatibilityManager.ClearActiveArtifact();
  }

  private void HandleThoughtEquipped(int slotIndex, Thought thought)
  {
    if (slotIndex < slots.Length)
      slots[slotIndex].SetData(thought);
  }

  private void HandleThoughtUnequipped(int slotIndex)
  {
    if (slotIndex < slots.Length)
      slots[slotIndex].Clear();
  }
}
