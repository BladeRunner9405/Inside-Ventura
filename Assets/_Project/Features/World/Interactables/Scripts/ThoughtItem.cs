using CherryFramework.DependencyManager;
using UnityEngine;

namespace InsideVentura.World.v1
{
  public class ThoughtItem : Item
  {
    [SerializeField]
    private Thought thoughtData;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Inject]
    private ThoughtItemTooltip _worldTooltip;

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
      _worldTooltip.Hide();
    }

    public override void SetFocused(bool active)
    {
      base.SetFocused(active);

      if (_worldTooltip == null || thoughtData == null)
        return;

      if (active)
      {
        _worldTooltip.Show(thoughtData, transform.position);
      }
      else
      {
        _worldTooltip.Hide();
      }
    }
  }
}
