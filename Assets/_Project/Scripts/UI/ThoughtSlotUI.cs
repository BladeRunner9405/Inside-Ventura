using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CherryFramework.UI.InteractiveElements.Populators;

public class ThoughtSlotUI : PopulatorElementBase<Thought>, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  [SerializeField] private Image iconImage;
  // [SerializeField] private Image backgroundImage;

  public ThoughtBag SourceBag { get; set; }
  public int SlotIndex { get; set; }
  public Artifact SourceArtifact { get; set; }
  public int ArtifactSlotIndex { get; set; } = -1;

  private RectTransform rectTransform;
  private CanvasGroup canvasGroup;

  public Thought CurrentThought => data;

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
    if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
  }

  public override void SetData(Thought data)
  {
    base.SetData(data);
    UpdateVisual();
  }

  private void UpdateVisual()
  {
    if (data != null)
    {
      iconImage.sprite = data.InventoryIcon;
      iconImage.enabled = true;
    }
    else
    {
      iconImage.enabled = false;
      // backgroundImage.color = Color.gray;
    }
  }

  public void Clear() => SetData(null);

  public void OnBeginDrag(PointerEventData eventData)
  {
    if (data == null) return;
    DragDropManager.Instance.StartDrag(this, eventData);
    canvasGroup.alpha = 0.6f;
    canvasGroup.blocksRaycasts = false;
  }

  public void OnDrag(PointerEventData eventData) => DragDropManager.Instance.OnDrag(eventData);

  public void OnEndDrag(PointerEventData eventData)
  {
    DragDropManager.Instance.EndDrag(eventData);
    canvasGroup.alpha = 1f;
    canvasGroup.blocksRaycasts = true;
  }
}
