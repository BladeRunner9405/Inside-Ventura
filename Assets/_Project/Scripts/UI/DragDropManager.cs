using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance { get; private set; }

    private ThoughtSlotUI dragSourceSlot;
    private GameObject dragObject;
    private RectTransform dragRectTransform;
    private Canvas dragCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDrag(ThoughtSlotUI sourceSlot, PointerEventData eventData)
    {
        dragSourceSlot = sourceSlot;

        // Находим Canvas верхнего уровня для визуального клона
        dragCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
        if (dragCanvas == null) dragCanvas = FindObjectOfType<Canvas>();

        // Создаём визуальный объект
        dragObject = new GameObject("DragThought");
        dragObject.transform.SetParent(dragCanvas.transform, false);
        dragObject.transform.SetAsLastSibling();

        var image = dragObject.AddComponent<Image>();
        image.sprite = sourceSlot.data.InventoryIcon;
        image.raycastTarget = false;
        image.SetNativeSize();

        dragRectTransform = dragObject.GetComponent<RectTransform>();
        dragRectTransform.sizeDelta = sourceSlot.GetComponent<RectTransform>().sizeDelta;

        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData) => UpdateDragPosition(eventData);

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (dragObject == null) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);
        dragRectTransform.anchoredPosition = pos;
    }

    public void EndDrag(PointerEventData eventData)
    {
        if (dragSourceSlot == null) return;

        // Поиск слота под курсором
        ThoughtSlotUI targetSlot = null;
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<ThoughtSlotUI>();
        }

        bool success = false;
        if (targetSlot != null && targetSlot != dragSourceSlot)
        {
            success = TryTransferThought(dragSourceSlot, targetSlot);
        }

        // Очистка
        if (dragObject != null) Destroy(dragObject);
        dragSourceSlot = null;
    }

    private bool TryTransferThought(ThoughtSlotUI source, ThoughtSlotUI target)
    {
        Thought thoughtToMove = source.data;
        if (thoughtToMove == null) return false;

        // Проверка совместимости с целевым артефактом (если целевой слот принадлежит артефакту)
        if (target.SourceArtifact != null && !IsThoughtCompatibleWithArtifact(thoughtToMove, target.SourceArtifact))
            return false;

        Thought thoughtAtTarget = target.data;

        // Удаляем из источника
        if (source.SourceBag != null)
            source.SourceBag.RemoveThought(thoughtToMove);
        else if (source.SourceArtifact != null)
            source.SourceArtifact.UnequipThought(source.ArtifactSlotIndex);

        // Добавляем в цель
        if (target.SourceBag != null)
            target.SourceBag.AddThought(thoughtToMove);
        else if (target.SourceArtifact != null)
            target.SourceArtifact.EquipThought(thoughtToMove, target.ArtifactSlotIndex);

        // Если в цели была мысль, перемещаем её в источник
        if (thoughtAtTarget != null)
        {
            if (source.SourceBag != null)
                source.SourceBag.AddThought(thoughtAtTarget);
            else if (source.SourceArtifact != null)
                source.SourceArtifact.EquipThought(thoughtAtTarget, source.ArtifactSlotIndex);
        }

        return true;
    }

    private bool IsThoughtCompatibleWithArtifact(Thought thought, Artifact artifact)
    {
        // Реализуйте логику проверки типов
        // Например, Weapon может принимать Weapon, Fluid, Absolute
        // Heart – Heart, Fluid, Absolute
        // Accessory – Accessory, Fluid, Absolute
        switch (artifact)
        {
            case Weapon _:
                return thought.Type == ThoughtType.Weapon ||
                       thought.Type == ThoughtType.Fluid ||
                       thought.Type == ThoughtType.Absolute;
            case Heart _:
                return thought.Type == ThoughtType.Heart ||
                       thought.Type == ThoughtType.Fluid ||
                       thought.Type == ThoughtType.Absolute;
            case Accessory _:
                return thought.Type == ThoughtType.Accessory ||
                       thought.Type == ThoughtType.Fluid ||
                       thought.Type == ThoughtType.Absolute;
            default:
                return false;
        }
    }
}
