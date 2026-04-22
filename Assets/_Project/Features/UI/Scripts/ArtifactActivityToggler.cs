using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactActivityToggler : InjectMonoBehaviour
{
  private Button _button;
  private EventTrigger _trigger;
  private ArtifactSlotsUI _artifactSlotsUI;

  [Inject]
  private ThoughtsCompatibilityManager _thoughtsCompatibilityManager;

  private void Awake() {
    _button = GetComponentInParent<Button>();
    _trigger = GetComponentInParent<EventTrigger>();

    _artifactSlotsUI = GetComponent<ArtifactSlotsUI>();
  }

  private void Start() {
    _thoughtsCompatibilityManager.OnActiveArtifactsChanged += Toggle;
  }

  private void Toggle() {
    if (_thoughtsCompatibilityManager.IfNoActiveArtifacts()) {
      _button.interactable = true;
      _trigger.enabled = true;
      return;
    }

    if (_thoughtsCompatibilityManager.ContainsArtifact(_artifactSlotsUI.ArtifactInstance)) return;

    _button.interactable = !_button.interactable;
    _trigger.enabled = !_trigger.enabled;
  }
}
