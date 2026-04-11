using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using UnityEngine;

public class ThoughtInventoryPresenter : PresenterBase
{
  [SerializeField]
  private ThoughtSlotUI slotPrefab;

  [SerializeField]
  private Transform slotsRoot;

  private ThoughtBag _bag;

  [Inject]
  private PlayerAccessor _playerAccessor;
  private ThoughtSlotUI[] _slots;

  protected override void OnDestroy()
  {
    if (_bag != null)
      _bag.OnThoughtsChanged -= RefreshDisplay;

    base.OnDestroy();
  }

  protected override void OnPresenterInitialized()
  {
    base.OnPresenterInitialized();

    _bag = _playerAccessor.Inventory.ThoughtBag;
    var slotCount = _bag.MaxSize;

    _slots = new ThoughtSlotUI[slotCount];
    for (var i = 0; i < slotCount; ++i)
    {
      var slot = Instantiate(slotPrefab, slotsRoot, true);
      slot.SourceBag = _bag;

      // ИСПОЛЬЗУЕМ НОВЫЕ ИМЕНА СВОЙСТВ
      slot.SourceArtifactInstance = null;
      slot.ArtifactSlotIndex = i;

      _slots[i] = slot;
    }

    _bag.OnThoughtsChanged += RefreshDisplay;
    RefreshDisplay();
  }

  private void RefreshDisplay()
  {
    var thoughts = _bag.Thoughts;
    for (var i = 0; i < _slots.Length; ++i)
      _slots[i].SetData(i < thoughts.Count ? thoughts[i] : null);
  }
}
