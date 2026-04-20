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

  public override void ShowTooltip()
  {
    if (_globalTooltip == null || _slot.CurrentThought == null || !_slot.IsCompatibleWithActiveArtifact())
      return;

    if (outline != null)
      outline.SetActive(true);

    var thought = _slot.CurrentThought;

    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);
    string tooltipText = BuildTooltipText(_slot.CurrentThought);
    tooltip?.SetText(tooltipText);

    _globalTooltip.SetText($"<b><u>{thought.Name}</u></b>\n{thought.Description}");
  }

  public static string BuildTooltipText(Thought thought)
  {
    string result = $"<b><u><color=white>{thought.Name}</color></u></b>\n";

    foreach (var effect in thought.Effects)
    {
      string color = effect.Type switch
      {
        EffectType.Positive => "green",
        EffectType.Negative => "red",
        EffectType.Neutral => "grey",
        _ => "white"
      };
      result += $"<color={color}>{effect.Description}</color>\n";
    }

    return result.TrimEnd('\n');
  }
}
