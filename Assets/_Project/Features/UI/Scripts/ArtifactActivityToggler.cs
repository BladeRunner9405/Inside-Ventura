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
    _thoughtsCompatibilityManager.OnActiveArtifactChanged += Toggle;
  }

  private void Toggle(ArtifactInstance artifactInstance) {
    if (_thoughtsCompatibilityManager.ActiveArtifact == null) {
      _button.interactable = true;
      _trigger.enabled = true;
      return;
    }

    if (_artifactSlotsUI.ArtifactInstance == artifactInstance) return;

    _button.interactable = !_button.interactable;
    _trigger.enabled = !_trigger.enabled;
  }
}
