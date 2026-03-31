using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Populators;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThoughtSlotUI : PopulatorElementBase<Thought>,
  IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
  [SerializeField] private Image iconImage;

  // Опционально: спрайт-заглушка для пустого слота (можно не назначать)
  [SerializeField] private Sprite emptySlotSprite;

  private CanvasGroup _canvasGroup;

  [Inject] private DragDropManager _dragDropManager;

  public ThoughtBag SourceBag { get; set; }
  public Artifact SourceArtifact { get; set; }
  public int SlotIndex { get; set; } = -1;
  public int ArtifactSlotIndex { get; set; } = -1;

  public Thought CurrentThought => data;

  private void Awake() {
    _canvasGroup = GetComponent<CanvasGroup>()
                   ?? gameObject.AddComponent<CanvasGroup>();

    DependencyContainer.Instance.InjectDependencies(this);
  }

  // ─── Сброс ───────────────────────────────────────────────────────────────

  private void OnDisable() {
    SourceBag = null;
    SourceArtifact = null;
    SlotIndex = -1;
    ArtifactSlotIndex = -1;
  }

  // ─── Drag & Drop ─────────────────────────────────────────────────────────

  public void OnBeginDrag(PointerEventData eventData) {
    if (data == null || _dragDropManager == null) return;

    _dragDropManager.StartDrag(this, eventData);
    _canvasGroup.alpha = 0.4f;
    _canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData) {
    _dragDropManager?.OnDrag(eventData);
  }

  // IDropHandler нужен для корректной работы EventSystem:
  // когда blocksRaycasts источника выключен, цель должна иметь IDropHandler,
  // чтобы eventData.pointerEnter указывал на неё.
  public void OnDrop(PointerEventData eventData) {
  }

  public void OnEndDrag(PointerEventData eventData) {
    _dragDropManager?.EndDrag(eventData);
    _canvasGroup.alpha = 1f;
    _canvasGroup.blocksRaycasts = true;
  }

  // ─── Данные ──────────────────────────────────────────────────────────────

  public override void SetData(Thought thought) {
    base.SetData(thought);
    RefreshVisual();
  }

  public void Clear() {
    SetData(null);
  }

  private void RefreshVisual() {
    // Компонент Image ВСЕГДА включён — иначе пустой слот не принимает дроп,
    // так как Raycast не проходит через отключённый Image.
    if (data != null) {
      iconImage.sprite = data.InventoryIcon;
      iconImage.color = Color.white;
    }
    else {
      iconImage.sprite = emptySlotSprite;
      iconImage.color = emptySlotSprite != null ? Color.white : Color.clear;
    }
  }
}
