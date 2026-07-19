using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragVisual : MonoBehaviour
{
    [SerializeField] Image image;

    public void Initialize(Thought draggedThought) {
      image.sprite = draggedThought.InventoryIcon;
    }
}
