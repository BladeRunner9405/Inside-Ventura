using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TooltipBase : MonoBehaviour
{
  [Inject]
  protected TooltipText _globalTooltip;

  [SerializeField]
  protected TooltipText tooltip;
  [SerializeField]
  protected Image tooltipBackground;

  [SerializeField]
  protected GameObject outline;

  private bool _pinned;

  protected virtual void Awake()
  {
    DependencyContainer.Instance.InjectDependencies(this);
  }

  public void TogglePinned() {
    _pinned = !_pinned;

    if (_pinned) ShowTooltip();
    else HideTooltip();
  }

  public void OnPointerEnter() {
    ShowTooltip();
  }

  public abstract void ShowTooltip();

  public void OnPointerExit() {
    if (_pinned) return;
    HideTooltip();
  }

  public void HideTooltip()
  {
    if (outline != null)
        outline.SetActive(false);

    tooltipBackground.color = Color.clear;
    tooltip?.Clear();
    _globalTooltip?.Clear();
  }
}
