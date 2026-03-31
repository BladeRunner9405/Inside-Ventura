using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using UnityEngine;

public class ThoughtInventoryPresenter : PresenterBase {
  [SerializeField] private ThoughtSlotUI slotPrefab;

  [SerializeField] private Transform slotsRoot;

  // Количество слотов совпадает с ThoughtBag.maxSize (или задаётся вручную)
  [SerializeField] private int slotCount = 20;

  private ThoughtBag _bag;

  [Inject] private PlayerAccessor _playerAccessor;
  private ThoughtSlotUI[] _slots;

  protected override void OnDestroy() {
    if (_bag != null)
      _bag.OnThoughtsChanged -= RefreshDisplay;

    base.OnDestroy();
  }

  protected override void OnPresenterInitialized() {
    base.OnPresenterInitialized();

    _bag = _playerAccessor.Inventory.ThoughtBag;

    // Создаём все слоты один раз — они не уничтожаются при изменении инвентаря
    _slots = new ThoughtSlotUI[slotCount];
    for (var i = 0; i < slotCount; i++) {
      var slot = Instantiate(slotPrefab, slotsRoot);
      slot.SourceBag = _bag;
      slot.SourceArtifact = null;
      slot.SlotIndex = i;
      _slots[i] = slot;
    }

    _bag.OnThoughtsChanged += RefreshDisplay;
    RefreshDisplay();
  }

  private void RefreshDisplay() {
    var thoughts = _bag.Thoughts;
    for (var i = 0; i < _slots.Length; i++)
      _slots[i].SetData(i < thoughts.Count ? thoughts[i] : null);
  }
}
