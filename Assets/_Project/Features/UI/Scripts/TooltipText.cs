using CherryFramework.DependencyManager;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TooltipText : InjectMonoBehaviour {
  private TextMeshProUGUI _text;

  protected override void OnEnable() {
    base.OnEnable();
    _text = GetComponent<TextMeshProUGUI>();

    if (!DependencyContainer.Instance.HasDependency<TooltipText>())
      DependencyContainer.Instance.BindAsSingleton(this);
  }

  public void SetText(string text) => _text.text = text;
  public void Clear() => _text.text = string.Empty;
}
