using UnityEngine;
using UnityEngine.EventSystems;

public class ThoughtTooltip : TooltipBase
{
  private ThoughtSlotUI _slot;

  protected override void Awake()
  {
    _slot = GetComponentInParent<ThoughtSlotUI>();
    base.Awake();
  }

  public override void OnPointerEnter(PointerEventData eventData)
  {
    if (_globalTooltip == null || _slot.CurrentThought == null)
      return;

    if (outline != null)
      outline.SetActive(true);

    var thought = _slot.CurrentThought;

    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);
    string tooltipText = BuildTooltipText(_slot.CurrentThought);
    tooltip?.SetText(tooltipText);

    _globalTooltip.SetText($"<b><u>{thought.Name}</u></b>\n{thought.Description}");
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
