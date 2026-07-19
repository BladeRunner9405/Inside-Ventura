using System;
using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactActivityToggler : InjectMonoBehaviour
{
  [SerializeField]
  private ArtifactSlotsUI artifactSlots;
  [SerializeField]
  private ArtifactTooltipUI artifactTooltip;

  private Button _button;
  private EventTrigger _trigger;

  [Inject]
  private ThoughtsCompatibilityManager _thoughtsCompatibilityManager;
  [Inject]
  private DragAndDropManager _dragAndDropManager;

  private void Awake() {
    _button = GetComponent<Button>();
    _trigger = GetComponent<EventTrigger>();
  }

  private void Start() {
    _thoughtsCompatibilityManager.OnActiveArtifactsChanged += ToggleActivity;
    _thoughtsCompatibilityManager.OnPinnedArtifactsCleared += ClearPinned;
  }

  private void OnDestroy() {
    _thoughtsCompatibilityManager.OnActiveArtifactsChanged -= ToggleActivity;
    _thoughtsCompatibilityManager.OnPinnedArtifactsCleared -= ClearPinned;
  }

  public void OnPointerEnter()
  {
    artifactSlots.SetAsActive();
    artifactSlots.IsPointerOnArtifact = true;

    artifactTooltip.OnPointerEnter();
  }
  public void OnPointerExit()
  {
    artifactSlots.UnsetAsActive();
    artifactSlots.IsPointerOnArtifact = false;

    artifactTooltip.OnPointerExit();
  }

  public void ClearPinned()
  {
    if (artifactSlots.Pinned)
    {
      TogglePinned();
    }
  }

  public void TogglePinned()
  {
    artifactSlots.Pinned = !artifactSlots.Pinned;

    artifactTooltip.TogglePinned();
  }

  private void ToggleActivity() {
    if (_dragAndDropManager.IsDragged) return;

    if (_thoughtsCompatibilityManager.IfNoActiveArtifacts()) {
      _button.interactable = true;
      _trigger.enabled = true;
      return;
    }

    if (_thoughtsCompatibilityManager.IsArtifactActive(artifactSlots.ArtifactInstance)) return;

    _button.interactable = !_button.interactable;
    _trigger.enabled = !_trigger.enabled;
  }
}
