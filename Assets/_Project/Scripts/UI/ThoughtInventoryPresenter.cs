using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using UnityEngine;

public class ThoughtInventoryPresenter : PresenterBase
{
  [SerializeField] private ThoughtSlotUI slotPrefab;
  [SerializeField] private Transform slotsRoot;

  [Inject] private PlayerAccessor _playerAccessor;

  private PlayerInventory playerInventory;
  private ThoughtPopulator populator;

  protected override void OnPresenterInitialized()
  {
    base.OnPresenterInitialized();
    playerInventory = _playerAccessor.Inventory;

    populator = new ThoughtPopulator(slotPrefab, slotsRoot);
    UpdateInventoryDisplay();

    playerInventory.ThoughtBag.OnThoughtsChanged += UpdateInventoryDisplay;
  }

  private void UpdateInventoryDisplay()
  {
    var thoughts = playerInventory.ThoughtBag.Thoughts;
    populator.UpdateElements(thoughts, 0.05f);

    int index = 0;
    foreach (var element in populator.Active)
    {
      if (element is ThoughtSlotUI slot)
      {
        slot.SourceBag = playerInventory.ThoughtBag;
        slot.SlotIndex = index;
      }
      index++;
    }
  }

  protected override void OnDestroy()
  {
    if (playerInventory?.ThoughtBag != null)
      playerInventory.ThoughtBag.OnThoughtsChanged -= UpdateInventoryDisplay;
    base.OnDestroy();
  }
}
