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

  [SerializeField]
  private Image mask;

  [SerializeField]
  private Image tooltip;

  [SerializeField]
  private ThoughtRemover remover;

  private CanvasGroup _canvasGroup;

  [Inject]
  private DragAndDropManager _dragDropManager;
  [Inject]
  private ThoughtsCompatibilityManager _thoughtsCompatibilityManager;

  // Ссылка на инвентарь (мешок), если слот принадлежит мешку
  public ThoughtBag SourceBag { get; set; }

  // Ссылка на "живой" инстанс артефакта, если слот принадлежит экипировке
  public ArtifactInstance SourceArtifactInstance { get; set; }

  public ThoughtRemover Remover => remover;

  // Индекс слота в артефакте
  public int ArtifactSlotIndex { get; set; } = -1;

  // Индекс слота в инвентаре
  public int BagSlotIndex { get; set; } = -1;

  // Текущая мысль (берет значение data из базового класса PopulatorElementBase)
  public Thought CurrentThought => data;

  private void Awake()
  {
    _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

    DependencyContainer.Instance.InjectDependencies(this);
  }

  protected override void OnEnable()
  {
    base.OnEnable();

    _thoughtsCompatibilityManager.OnActiveArtifactsChanged += OnActiveArtifactChanged;
    RefreshVisual();
  }

  private void OnDisable()
  {
    _thoughtsCompatibilityManager.OnActiveArtifactsChanged -= OnActiveArtifactChanged;
  }

  private void OnActiveArtifactChanged() => RefreshVisual();

  public bool IsCompatibleWithActiveArtifacts()
  {
    if (!data) return true;

    if (_thoughtsCompatibilityManager.IfNoActiveArtifacts()) return true;

    foreach (var activeArtifact in _thoughtsCompatibilityManager.ActiveArtifacts) {
      if (data.HasRightType(activeArtifact.BaseData)) {
        return true;
      }
    }
    return false;
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (!data || !_dragDropManager || !IsCompatibleWithActiveArtifacts())
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
    if (data)
    {
      iconImage.sprite = data.InventoryIcon;
      iconImage.color = IsCompatibleWithActiveArtifacts() ? Color.white : new Color(1, 1, 1, 0.3f);
      if (mask) mask.color = Color.white;

      tooltip.raycastTarget = true;
    }
    else
    {
      iconImage.sprite = null;
      iconImage.color = Color.clear;
      if (mask) mask.color = Color.clear;

      tooltip.raycastTarget = false;
    }
  }

  public void SetActiveArtifacts() {
    if (!data) return;

    _thoughtsCompatibilityManager.ActivateForThought(data.Type);
  }

  public void UnsetActiveArtifacts() {
    if (!data) return;

    _thoughtsCompatibilityManager.DeactivateForThoughts();
  }
}
