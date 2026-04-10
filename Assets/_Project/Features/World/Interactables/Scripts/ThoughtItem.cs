using UnityEngine;

namespace InsideVentura.World
{
  public class ThoughtItem : Item
  {
    [SerializeField]
    private Thought thoughtData;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
      UpdateVisual();
    }

    public void Setup(Thought thought)
    {
      thoughtData = thought;
      UpdateVisual();
    }

    private void UpdateVisual()
    {
      if (spriteRenderer != null && thoughtData != null && thoughtData.InventoryIcon != null)
      {
        spriteRenderer.sprite = thoughtData.InventoryIcon;
      }
    }

    protected override bool CanPickUp()
    {
      if (!PlayerAccessor.Inventory)
        return false;

      return PlayerAccessor.Inventory.CanAddThought();
    }

    protected override void OnPickup()
    {
      if (!PlayerAccessor.Inventory)
        return;

      PlayerAccessor.Inventory.AddThoughtToBag(thoughtData);
    }
  }
}
