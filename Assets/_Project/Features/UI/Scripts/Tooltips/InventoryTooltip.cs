public class InventoryTooltip : TooltipBase
{
  private ThoughtBag _thoughtBag;

  public void Initialize(ThoughtBag thoughtBag)
  {
    _thoughtBag = thoughtBag;
  }

  public override void ShowTooltip()
  {
    if (_thoughtBag == null)
      return;

    base.ShowTooltip();
  }

  public override void SetTooltipText() {
    tooltipText.text = BuildTooltipText(_thoughtBag);

    // _globalTooltip.SetText($"<b><u>{_thoughtBag.Name}</u></b>");
  }

  private string BuildTooltipText(ThoughtBag thoughtBag)
  {
    string result = $"<b><u><color=white>{thoughtBag.Name}</color></u></b>\n";
    result += $"<color=#CCCCCC>{thoughtBag.Description}</color>\n\n";

    return result.TrimEnd('\n');
  }
}
