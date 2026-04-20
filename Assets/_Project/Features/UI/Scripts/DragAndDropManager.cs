using CherryFramework.DependencyManager;
using InsideVentura.World.v1;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropManager : InjectMonoBehaviour
{
    [SerializeField]
    private Canvas rootCanvas;

    [SerializeField]
    private GameObject dragVisualPrefab;

    // [SerializeField]
    // private EncounterManager encounterManager;

    private Thought _draggedThought;

    private GameObject _dragVisual;
    private RectTransform _dragVisualRect;
    private ThoughtSlotUI _sourceSlot;

    public void StartDrag(ThoughtSlotUI sourceSlot, PointerEventData eventData)
    {
      // DEPRECATED.
      // if (encounterManager.IsEncounterActive)
      // {
      //   return;
      // }

      if (EncounterStatus.Active) {
        return;
      }

      _sourceSlot = sourceSlot;
      _draggedThought = sourceSlot.CurrentThought;

      CreateDragVisual(sourceSlot);
      MoveDragVisual(eventData);
    }

    public void OnDrag(PointerEventData eventData) => MoveDragVisual(eventData);

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
          return thought.HasRightType(targetSlot.SourceArtifactInstance.BaseData);
        return false;
    }

    private void TryTransferThought(ThoughtSlotUI source, ThoughtSlotUI target)
    {
        if (_draggedThought == null) return;

        Thought sourceThought = source.CurrentThought;
        Thought targetThought = target.CurrentThought;

        if (target.SourceArtifactInstance != null && !IsCompatible(sourceThought, target.SourceArtifactInstance))
        {
            Debug.Log("[DragDrop] Мысль несовместима с артефактом.");
            return;
        }

        // Если двигаем из артефакта в инвентарь
        if (source.SourceArtifactInstance != null && target.SourceBag != null)
        {
          // Если двигаем в слот с мыслью
          if (targetThought != null) {
            // Если эта мысль совместима с артефактом
            if (IsCompatible(targetThought, source.SourceArtifactInstance)) {
              SwapThoughts(source, target);
              return;
            }
            MoveThoughtFromArtifactToUncompatible(source, target);
            return;
          }
          MoveThoughtFromArtifactToEmpty(source, target);
          return;
        }

        SwapThoughts(source, target);
    }

    private void MoveThoughtFromArtifactToEmpty(ThoughtSlotUI source, ThoughtSlotUI target) {
      Thought sourceThought = source.CurrentThought;

      ThoughtBag bag = target.SourceBag;
      int targetIndex = target.BagSlotIndex;

      bag.SetThoughtAt(targetIndex, sourceThought);
      source.SourceArtifactInstance.UnequipThought(source.ArtifactSlotIndex);
    }

    private void MoveThoughtFromArtifactToUncompatible(ThoughtSlotUI source, ThoughtSlotUI target) {
      Thought sourceThought = source.CurrentThought;
      Thought targetThought = target.CurrentThought;

      ThoughtBag bag = target.SourceBag;
      int targetIndex = target.BagSlotIndex;

      int nearestFreeIndex = -1;
      int minDistance = int.MaxValue;
      for (int i = 0; i < bag.MaxSize; ++i)
      {
        if (bag.Thoughts[i] == null)
        {
          int distance = Mathf.Abs(i - targetIndex);
          if (distance < minDistance)
          {
            minDistance = distance;
            nearestFreeIndex = i;
          }
        }
      }

      if (nearestFreeIndex != -1)
      {
        bag.SetThoughtAt(nearestFreeIndex, targetThought);
        bag.SetThoughtAt(targetIndex, sourceThought);
        source.SourceArtifactInstance.UnequipThought(source.ArtifactSlotIndex);
      }
      else
      {
        Debug.LogWarning("[DragDrop] Нет свободных ячеек в инвентаре для сдвига!");
      }
    }

    private void SwapThoughts(ThoughtSlotUI source, ThoughtSlotUI target) {
      Thought sourceThought = source.CurrentThought;
      Thought targetThought = target.CurrentThought;

      if (source.SourceBag != null)
        source.SourceBag.SetThoughtAt(source.BagSlotIndex, targetThought);
      else if (source.SourceArtifactInstance != null)
        source.SourceArtifactInstance.EquipThought(targetThought, source.ArtifactSlotIndex);

      if (target.SourceBag != null)
        target.SourceBag.SetThoughtAt(target.BagSlotIndex, sourceThought);
      else if (target.SourceArtifactInstance != null)
        target.SourceArtifactInstance.EquipThought(sourceThought, target.ArtifactSlotIndex);
    }

    private static bool IsCompatible(Thought thought, ArtifactInstance artifact)
        => thought.HasRightType(artifact.BaseData);

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
            _dragVisualRect.sizeDelta = sourceSlot.GetComponent<RectTransform>().sizeDelta;
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
}
