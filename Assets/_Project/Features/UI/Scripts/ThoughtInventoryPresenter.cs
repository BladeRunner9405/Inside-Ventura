using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using TMPro;
using UnityEngine;

public class ThoughtInventoryPresenter : PresenterBase
{
  [SerializeField]
  private ThoughtSlotUI slotPrefab;

  [SerializeField]
  private Transform slotsRoot;

  [SerializeField]
  private TextMeshProUGUI bagCapacityText;
  [SerializeField]
  private InventoryTooltip inventoryTooltip;

  private ThoughtBag _bag;

  [Inject]
  private PlayerAccessor _playerAccessor;
  private ThoughtSlotUI[] _slots;

  protected override void OnDestroy()
  {
    if (_bag != null)
      _bag.OnThoughtsChanged -= RefreshDisplay;

    _playerAccessor.OnPlayerRegistered -= OnPlayerRegistered;

    base.OnDestroy();
  }

  protected override void OnPresenterInitialized()
  {
    base.OnPresenterInitialized();

    _playerAccessor.OnPlayerRegistered += OnPlayerRegistered;

    if (_playerAccessor.HasPlayer) OnPlayerRegistered(null);
  }

  private void OnPlayerRegistered(Player player) {
    _bag = _playerAccessor.Inventory.ThoughtBag;
    var slotCount = _bag.MaxSize;

    _slots = new ThoughtSlotUI[slotCount];
    for (var i = 0; i < slotCount; ++i)
    {
      var slot = Instantiate(slotPrefab, slotsRoot, true);
      slot.transform.localScale = slotPrefab.transform.localScale;
      slot.SourceBag = _bag;

      // ИСПОЛЬЗУЕМ НОВЫЕ ИМЕНА СВОЙСТВ
      slot.SourceArtifactInstance = null;
      slot.ArtifactSlotIndex = -1;
      slot.BagSlotIndex = i;

      _slots[i] = slot;
    }

    if (inventoryTooltip != null)
      inventoryTooltip.Initialize(_bag);

    _bag.OnThoughtsChanged += RefreshDisplay;
    RefreshDisplay();
  }

  private void RefreshDisplay()
  {
    var thoughts = _bag.Thoughts;
    for (var i = 0; i < _slots.Length; ++i)
      _slots[i].SetData(i < thoughts.Count ? thoughts[i] : null);

    if (bagCapacityText != null)
    {
      int occupied = 0;
      for (int i = 0; i < thoughts.Count; ++i)
      {
        if (thoughts[i] != null) ++occupied;
      }
      bagCapacityText.text = $"{occupied}/{_bag.MaxSize}";
    }
  }
}
