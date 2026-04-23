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

  public bool IsPointerOnArtifact { get; set; }

  public ArtifactInstance ArtifactInstance { get; private set; }

  public void Initialize(ArtifactInstance artifactInstance) {
    // Защита: если мы уже инициализированы этим артефактом, ничего не делаем
    if (ArtifactInstance == artifactInstance)
      return;

    // Защита: отписываемся от старого артефакта, если он был
    if (ArtifactInstance != null) {
      ArtifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
      ArtifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;
    }

    ArtifactInstance = artifactInstance;

    if (artifactTooltip != null)
      artifactTooltip.Initialize(artifactInstance);

    var count = Mathf.Min(slots.Length, artifactInstance.BaseData.SlotsCount);
    for (var i = 0; i < count; ++i) {
      var slot = slots[i];
      slot.SourceBag = null;
      slot.SourceArtifactInstance = artifactInstance;
      slot.ArtifactSlotIndex = i;
      slot.SetData(artifactInstance.EquippedThoughts[i]);
    }

    ArtifactInstance.OnThoughtEquipped += HandleThoughtEquipped;
    ArtifactInstance.OnThoughtUnequipped += HandleThoughtUnequipped;
  }

  private void Start() {
    _thoughtsCompatibilityManager.OnThoughtDeselected += HandleThoughtDeselected;
  }

  private void OnDestroy()
  {
    if (ArtifactInstance == null)
      return;

    ArtifactInstance.OnThoughtEquipped -= HandleThoughtEquipped;
    ArtifactInstance.OnThoughtUnequipped -= HandleThoughtUnequipped;

    _thoughtsCompatibilityManager.OnThoughtDeselected -= HandleThoughtDeselected;
  }

  public void TogglePinned() => _pinned = !_pinned;

  public void SetAsActive() {
    if (_thoughtsCompatibilityManager.IfNoActiveArtifacts())
      _thoughtsCompatibilityManager.AddPinnedArtifact(ArtifactInstance);
  }

  public void UnsetAsActive()
  {
    if (!_pinned && _thoughtsCompatibilityManager.ContainsArtifact(ArtifactInstance))
      _thoughtsCompatibilityManager.RemovePinnedArtifact(ArtifactInstance);
  }

  private void HandleThoughtDeselected() {
    if (IsPointerOnArtifact) {
      SetAsActive();
    }
  }

  private void HandleThoughtEquipped(int slotIndex, Thought thought)
  {
    if (slotIndex < slots.Length) {
      slots[slotIndex].SetData(thought);
      artifactTooltip.SetTooltipText();
    }
  }

  private void HandleThoughtUnequipped(int slotIndex)
  {
    if (slotIndex < slots.Length) {
      slots[slotIndex].Clear();
      artifactTooltip.SetTooltipText();
    }
  }
}
