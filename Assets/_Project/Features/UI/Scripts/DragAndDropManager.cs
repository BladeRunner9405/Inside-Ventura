using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropManager : InjectMonoBehaviour
{
  [SerializeField]
  private Canvas rootCanvas;

  [SerializeField]
  private GameObject dragVisualPrefab;

  private Thought _draggedThought;

  private GameObject _dragVisual;
  private RectTransform _dragVisualRect;
  private ThoughtSlotUI _sourceSlot;

  public void StartDrag(ThoughtSlotUI sourceSlot, PointerEventData eventData)
  {
    _sourceSlot = sourceSlot;
    _draggedThought = sourceSlot.CurrentThought;

    CreateDragVisual(sourceSlot);
    MoveDragVisual(eventData);
  }

  public void OnDrag(PointerEventData eventData)
  {
    MoveDragVisual(eventData);
  }

  public void EndDrag(PointerEventData eventData)
  {
    if (_sourceSlot == null)
      return;

    ThoughtSlotUI targetSlot = FindNearestValidSlot(eventData);

    if (targetSlot != null && targetSlot != _sourceSlot)
      TryTransferThought(_sourceSlot, targetSlot);

    CleanUp();
  }

  private ThoughtSlotUI FindNearestValidSlot(PointerEventData eventData)
  {
    ThoughtSlotUI[] allSlots = FindObjectsByType<ThoughtSlotUI>(FindObjectsSortMode.None);
    ThoughtSlotUI nearest = null;
    float minSqrDistance = float.MaxValue;
    Vector2 screenPos = eventData.position;

    foreach (var slot in allSlots)
    {
      if (slot == _sourceSlot) continue;
      if (!slot.gameObject.activeInHierarchy) continue;
      if (!CanPlaceThought(slot, _draggedThought)) continue;

      RectTransform rect = slot.GetComponent<RectTransform>();
      Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, rect.position);
      float sqrDist = (slotScreenPos - screenPos).sqrMagnitude;
      if (sqrDist < minSqrDistance)
      {
        minSqrDistance = sqrDist;
        nearest = slot;
      }
    }
    return nearest;
  }

  private bool CanPlaceThought(ThoughtSlotUI targetSlot, Thought thought)
  {
    if (targetSlot.SourceBag != null)
      return true;

    if (targetSlot.SourceArtifactInstance != null)
      return IsCompatible(thought, targetSlot.SourceArtifactInstance);

    return false;
  }

  private bool TryTransferThought(ThoughtSlotUI source, ThoughtSlotUI target)
  {
    if (_draggedThought == null)
    {
      Debug.LogWarning("[DragDrop] Кешированная мысль null.");
      return false;
    }

    // Проверяем совместимость через инстанс
    if (
      target.SourceArtifactInstance != null
      && !IsCompatible(_draggedThought, target.SourceArtifactInstance)
    )
    {
      Debug.Log("[DragDrop] Мысль несовместима с артефактом.");
      return false;
    }

    var swapThought = target.CurrentThought;

    RemoveFromSource(source, _draggedThought);

    if (!AddToTarget(target, _draggedThought))
      return false;

    if (swapThought != null)
      AddToSource(source, swapThought);

    return true;
  }

  private void RemoveFromSource(ThoughtSlotUI source, Thought thought)
  {
    if (source.SourceBag != null)
      source.SourceBag.RemoveThought(thought);
    else if (source.SourceArtifactInstance != null)
      source.SourceArtifactInstance.UnequipThought(source.ArtifactSlotIndex);
  }

  private bool AddToTarget(ThoughtSlotUI target, Thought thought)
  {
    if (target.SourceBag != null)
    {
      target.SourceBag.AddThought(thought);
      return true;
    }

    if (target.SourceArtifactInstance != null)
    {
      target.SourceArtifactInstance.EquipThought(thought, target.ArtifactSlotIndex);
      return true;
    }

    Debug.LogError("[DragDrop] У цели нет ни мешка, ни артефакта.");
    return false;
  }

  private void AddToSource(ThoughtSlotUI source, Thought thought)
  {
    if (source.SourceBag != null)
      source.SourceBag.AddThought(thought);
    else if (source.SourceArtifactInstance != null)
      source.SourceArtifactInstance.EquipThought(thought, source.ArtifactSlotIndex);
  }

  private void CreateDragVisual(ThoughtSlotUI sourceSlot)
  {
    _dragVisual = Instantiate(dragVisualPrefab, rootCanvas.transform, false);
    _dragVisual.transform.SetAsLastSibling();

    var img = _dragVisual.transform.GetChild(0).GetChild(0).GetComponent<Image>(); // некрасиво
    if (img != null)
    {
      img.sprite = _draggedThought.InventoryIcon;
      img.raycastTarget = false;
    }

    _dragVisualRect = _dragVisual.GetComponent<RectTransform>();
    if (_dragVisualRect != null)
    {
      _dragVisualRect.sizeDelta = sourceSlot.GetComponent<RectTransform>().sizeDelta;
    }
  }

  private void MoveDragVisual(PointerEventData eventData)
  {
    if (_dragVisual == null)
      return;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      rootCanvas.transform as RectTransform,
      eventData.position,
      eventData.pressEventCamera,
      out var localPoint
    );

    _dragVisualRect.anchoredPosition = localPoint;
  }

  private void CleanUp()
  {
    if (_dragVisual != null)
      Destroy(_dragVisual);

    _dragVisual = null;
    _dragVisualRect = null;
    _sourceSlot = null;
    _draggedThought = null;
  }

  // Обновленный метод проверки совместимости
  private static bool IsCompatible(Thought thought, ArtifactInstance artifactInstance)
  {
    // Делегируем логику проверки самому классу Thought, проверяя базовые данные артефакта
    return thought.HasRightType(artifactInstance.BaseData);
  }
}
