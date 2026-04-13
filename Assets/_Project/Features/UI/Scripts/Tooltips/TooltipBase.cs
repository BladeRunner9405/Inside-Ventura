using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class TooltipBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [Inject]
  protected TooltipText _globalTooltip;

  [SerializeField]
  protected TooltipText tooltip;
  [SerializeField]
  protected Image tooltipBackground;

  [SerializeField]
  protected GameObject outline;

  protected virtual void Awake()
  {
    DependencyContainer.Instance.InjectDependencies(this);
  }

  public abstract void OnPointerEnter(PointerEventData eventData);

  public void OnPointerExit(PointerEventData eventData) {
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
