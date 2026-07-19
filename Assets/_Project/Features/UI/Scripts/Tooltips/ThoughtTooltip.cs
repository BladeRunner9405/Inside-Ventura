public class ThoughtTooltip : TooltipBase
{
  private ThoughtSlotUI _slot;

  protected void Awake()
  {
    _slot = GetComponentInParent<ThoughtSlotUI>();
    //base.Awake();
  }

  public override void ShowTooltip()
  {
    if (_slot.CurrentThought == null || !_slot.IsCompatibleWithActiveArtifacts())
      return;

    base.ShowTooltip();
  }

  public override void SetTooltipText() {
    tooltipText.text = BuildTooltipText(_slot.CurrentThought);

    // _globalTooltip.SetText($"<b><u>{thought.Name}</u></b>\n{thought.Description}");
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
