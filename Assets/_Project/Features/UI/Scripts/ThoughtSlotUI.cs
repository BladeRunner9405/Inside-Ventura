using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Populators;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThoughtSlotUI
  : PopulatorElementBase<Thought>,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
  [SerializeField]
  private Image iconImage;

  private CanvasGroup _canvasGroup;

  [Inject]
  private DragAndDropManager _dragDropManager;

  // Ссылка на инвентарь (мешок), если слот принадлежит мешку
  public ThoughtBag SourceBag { get; set; }

  // Ссылка на "живой" инстанс артефакта, если слот принадлежит экипировке
  public ArtifactInstance SourceArtifactInstance { get; set; }

  // Индекс слота в артефакте
  public int ArtifactSlotIndex { get; set; } = -1;

  // Текущая мысль (берет значение data из базового класса PopulatorElementBase)
  public Thought CurrentThought => data;

  private void Awake()
  {
    _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

    DependencyContainer.Instance.InjectDependencies(this);
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (data == null || _dragDropManager == null)
      return;

    _dragDropManager.StartDrag(this, eventData);
    _canvasGroup.alpha = 0.4f;
    _canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData)
  {
    _dragDropManager?.OnDrag(eventData);
  }

  public void OnDrop(PointerEventData eventData)
  {
    // Логика Drop обрабатывается менеджером в OnEndDrag
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    _dragDropManager?.EndDrag(eventData);
    _canvasGroup.alpha = 1f;
    _canvasGroup.blocksRaycasts = true;
  }

  public override void SetData(Thought thought)
  {
    base.SetData(thought);
    RefreshVisual();
  }

  public void Clear()
  {
    SetData(null);
  }

  private void RefreshVisual()
  {
    if (data != null)
    {
      iconImage.sprite = data.InventoryIcon;
      iconImage.color = Color.white;
    }
    else
    {
      iconImage.sprite = null;
      iconImage.color = Color.clear;
    }
  }
}
