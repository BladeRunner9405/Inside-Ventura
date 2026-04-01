using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ThoughtSlotUI))]
public class ThoughtTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  [Inject] private TooltipText _tooltip;
  private ThoughtSlotUI _slot;

  private void Awake() {
    _slot = GetComponent<ThoughtSlotUI>();
    DependencyContainer.Instance.InjectDependencies(this);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (_tooltip == null || _slot.CurrentThought == null) return;

    var thought = _slot.CurrentThought;
    _tooltip.SetText($"<b>{thought.Name} ({thought.Type})</b>\n{thought.Description}");
  }

  public void OnPointerExit(PointerEventData eventData) {
    _tooltip?.Clear();
  }
}
