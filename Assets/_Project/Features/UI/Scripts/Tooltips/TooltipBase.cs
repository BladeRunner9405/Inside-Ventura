using CherryFramework.DependencyManager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TooltipBase : MonoBehaviour
{
  //[Inject]
  //protected TooltipText _globalTooltip;

  [SerializeField]
  protected TextMeshProUGUI tooltipText;
  [SerializeField]
  protected Image tooltipBackground;

  [SerializeField]
  protected GameObject outline;

  private bool _pinned;

  /*protected virtual void Awake()
  {
    DependencyContainer.Instance.InjectDependencies(this);
  }*/

  public void TogglePinned() {
    _pinned = !_pinned;

    if (_pinned) ShowTooltip();
    else HideTooltip();
  }

  public void OnPointerEnter() {
    ShowTooltip();
  }

  public virtual void ShowTooltip() {
    tooltipBackground.color = new Color(0f, 0f, 0f, 0.75f);
    tooltipText.enabled = true;

    SetTooltipText();

    if (outline != null)
      outline.SetActive(true);
  }

  public abstract void SetTooltipText();

  public void OnPointerExit() {
    if (_pinned) return;
    HideTooltip();
  }

  public void HideTooltip()
  {
    if (outline != null)
        outline.SetActive(false);

    tooltipBackground.color = Color.clear;
    tooltipText.enabled = false;

    // _globalTooltip?.Clear();
  }
}
