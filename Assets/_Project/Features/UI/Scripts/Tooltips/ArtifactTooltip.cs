public class ArtifactTooltip : TooltipBase
{
  private ArtifactInstance _artifactInstance;

  public void Initialize(ArtifactInstance artifactInstance)
  {
    _artifactInstance = artifactInstance;
  }

  public override void ShowTooltip()
  {
    if (_artifactInstance == null)
      return;

    base.ShowTooltip();
  }

  public override void SetTooltipText() {
    tooltipText.text = BuildTooltipText(_artifactInstance);

    // _globalTooltip.SetText($"<b><u>{_artifactInstance.BaseData.Name}</u></b>");
  }

  private string BuildTooltipText(ArtifactInstance artifact)
  {
    var baseData = artifact.BaseData;
    string result = $"<b><u><color=white>{baseData.Name}</color></u></b>\n";
    result += $"<color=#CCCCCC>{baseData.Description}</color>\n\n";

    result += "<b>Экипированные мысли:</b>\n";
    var thoughts = artifact.EquippedThoughts;
    bool hasAny = false;
    for (int i = 0; i < thoughts.Length; ++i)
    {
      var thought = thoughts[i];
      if (thought != null)
      {
        hasAny = true;
        result += $"- {thought.Name}\n";
      }
    }
    if (!hasAny)
      result += "<i>404</i>";

    return result.TrimEnd('\n');
  }
}
