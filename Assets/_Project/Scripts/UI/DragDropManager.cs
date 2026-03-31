using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropManager : InjectMonoBehaviour {
  private Thought _draggedThought;

  private GameObject _dragVisual;
  private RectTransform _dragVisualRect;
  private Canvas _rootCanvas;
  private ThoughtSlotUI _sourceSlot;

  protected override void OnEnable() {
    base.OnEnable();
    DependencyContainer.Instance.BindAsSingleton(this);
  }

  public void StartDrag(ThoughtSlotUI sourceSlot, PointerEventData eventData) {
    _sourceSlot = sourceSlot;
    _draggedThought = sourceSlot.CurrentThought;

    _rootCanvas = FindRootCanvas();
    if (_rootCanvas == null) return;

    CreateDragVisual(sourceSlot);
    MoveDragVisual(eventData);
  }

  public void OnDrag(PointerEventData eventData) {
    MoveDragVisual(eventData);
  }

  public void EndDrag(PointerEventData eventData) {
    if (_sourceSlot == null) return;

    ThoughtSlotUI targetSlot = null;

    var entered = eventData.pointerEnter;
    if (entered != null)
      targetSlot = entered.GetComponentInParent<ThoughtSlotUI>();

    if (targetSlot != null && targetSlot != _sourceSlot)
      TryTransferThought(_sourceSlot, targetSlot);

    CleanUp();
  }

  private bool TryTransferThought(ThoughtSlotUI source, ThoughtSlotUI target) {
    if (_draggedThought == null) {
      Debug.LogWarning("[DragDrop] Кешированная мысль null.");
      return false;
    }

    if (target.SourceArtifact != null && !IsCompatible(_draggedThought, target.SourceArtifact)) {
      Debug.Log("[DragDrop] Мысль несовместима с артефактом.");
      return false;
    }

    // Сохраняем мысль цели до изменений (она тоже может обнулиться при swap)
    var swapThought = target.CurrentThought;

    // 1. Убираем из источника
    RemoveFromSource(source, _draggedThought);

    // 2. Кладём в цель
    if (!AddToTarget(target, _draggedThought))
      return false;

    // 3. Swap: если в цели была мысль — возвращаем её в источник
    if (swapThought != null)
      AddToSource(source, swapThought);

    return true;
  }

  private void RemoveFromSource(ThoughtSlotUI source, Thought thought) {
    if (source.SourceBag != null)
      source.SourceBag.RemoveThought(thought);
    else if (source.SourceArtifact != null)
      source.SourceArtifact.UnequipThought(source.ArtifactSlotIndex);
  }

  private bool AddToTarget(ThoughtSlotUI target, Thought thought) {
    if (target.SourceBag != null) {
      target.SourceBag.AddThought(thought);
      return true;
    }

    if (target.SourceArtifact != null) {
      target.SourceArtifact.EquipThought(thought, target.ArtifactSlotIndex);
      return true;
    }

    Debug.LogError("[DragDrop] У цели нет ни мешка, ни артефакта.");
    return false;
  }

  private void AddToSource(ThoughtSlotUI source, Thought thought) {
    if (source.SourceBag != null)
      source.SourceBag.AddThought(thought);
    else if (source.SourceArtifact != null)
      source.SourceArtifact.EquipThought(thought, source.ArtifactSlotIndex);
  }

  // ─── Визуал ──────────────────────────────────────────────────────────────

  private void CreateDragVisual(ThoughtSlotUI sourceSlot) {
    _dragVisual = new GameObject("DragVisual_Thought");
    _dragVisual.transform.SetParent(_rootCanvas.transform, false);
    _dragVisual.transform.SetAsLastSibling();

    var img = _dragVisual.AddComponent<Image>();
    img.sprite = _draggedThought.InventoryIcon;
    img.raycastTarget = false;

    _dragVisualRect = _dragVisual.GetComponent<RectTransform>();
    _dragVisualRect.sizeDelta = sourceSlot.GetComponent<RectTransform>().sizeDelta;
  }

  private void MoveDragVisual(PointerEventData eventData) {
    if (_dragVisual == null) return;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      _rootCanvas.transform as RectTransform,
      eventData.position,
      eventData.pressEventCamera,
      out var localPoint);

    _dragVisualRect.anchoredPosition = localPoint;
  }

  private void CleanUp() {
    if (_dragVisual != null) Destroy(_dragVisual);
    _dragVisual = null;
    _dragVisualRect = null;
    _sourceSlot = null;
    _draggedThought = null;
  }

  // ─── Совместимость ───────────────────────────────────────────────────────

  private static bool IsCompatible(Thought thought, Artifact artifact) {
    if (thought.Type is ThoughtType.Fluid or ThoughtType.Absolute)
      return true;

    return artifact switch {
      Weapon => thought.Type == ThoughtType.Weapon,
      Heart => thought.Type == ThoughtType.Heart,
      Accessory => thought.Type == ThoughtType.Accessory,
      _ => false
    };
  }

  // ─── Вспомогательное ─────────────────────────────────────────────────────

  private Canvas FindRootCanvas() {
    var canvas = GetComponentInParent<Canvas>()?.rootCanvas
                 ?? FindObjectOfType<Canvas>();

    if (canvas == null) Debug.LogError("[DragDrop] Canvas не найден.");
    return canvas;
  }
}
