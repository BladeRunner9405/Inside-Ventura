using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThoughtTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [Inject]
  private TooltipText _globalTooltip;

  private ThoughtSlotUI _slot;

  [SerializeField]
  private TooltipText tooltip;
  [SerializeField]
  private Image tooltipBackground;

  private void Awake()
  {
    _slot = GetComponentInParent<ThoughtSlotUI>();
    DependencyContainer.Instance.InjectDependencies(this);
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (_globalTooltip == null || _slot.CurrentThought == null)
      return;

    if (_slot.Outline != null)
      _slot.Outline.SetActive(true);

    var thought = _slot.CurrentThought;

    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);
    string tooltipText = BuildTooltipText(_slot.CurrentThought);
    tooltip?.SetText(tooltipText);

    _globalTooltip.SetText($"<b><u>{thought.Name}</u></b>\n{thought.Description}");
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (_slot.Outline != null)
      _slot.Outline.SetActive(false);

    tooltipBackground.color = Color.clear;
    tooltip?.Clear();
    _globalTooltip?.Clear();
  }

  private string BuildTooltipText(Thought thought)
  {
    string result = $"<b><u><color=white>{thought.Name}</color></u></b>\n";

    foreach (var effect in thought.Effects)
    {
      string color;
      switch (effect.Type)
      {
        case EffectType.Positive:
          color = "green";
          break;
        case EffectType.Negative:
          color = "red";
          break;
        case EffectType.Neutral:
          color = "grey";
          break;
        default:
          color = "white";
          break;
      }
      result += $"<color={color}>{effect.Description}</color>\n";
    }

    return result.TrimEnd('\n');
  }
}
