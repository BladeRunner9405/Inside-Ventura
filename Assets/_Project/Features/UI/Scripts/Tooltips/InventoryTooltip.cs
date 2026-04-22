using UnityEngine;
using UnityEngine.EventSystems;

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

    if (outline != null)
      outline.SetActive(true);

    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);
    string tooltipText = BuildTooltipText(_thoughtBag);
    tooltip?.SetText(tooltipText);

    // _globalTooltip.SetText($"<b><u>{_thoughtBag.Name}</u></b>");
  }

  private string BuildTooltipText(ThoughtBag thoughtBag)
  {
    string result = $"<b><u><color=white>{thoughtBag.Name}</color></u></b>\n";
    result += $"<color=#CCCCCC>{thoughtBag.Description}</color>\n\n";

    return result.TrimEnd('\n');
  }
}
