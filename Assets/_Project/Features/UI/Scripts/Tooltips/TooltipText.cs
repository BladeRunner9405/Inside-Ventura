using CherryFramework.DependencyManager;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TooltipText : InjectMonoBehaviour
{
  private TextMeshProUGUI _text;

  private void Awake() {
    _text = GetComponent<TextMeshProUGUI>();
  }

  protected override void OnEnable()
  {
    base.OnEnable();

    if (!DependencyContainer.Instance.HasDependency<TooltipText>())
      DependencyContainer.Instance.BindAsSingleton(this);
  }

  public void SetText(string text) => _text.text = text;

  public void Clear() {
    if (!_text) return;
    _text.text = string.Empty;
  }
}
