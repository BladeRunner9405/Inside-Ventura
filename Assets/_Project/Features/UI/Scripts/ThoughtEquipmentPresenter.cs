using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using UnityEngine;

public class ThoughtEquipmentPresenter : PresenterBase
{
  [SerializeField]
  private ArtifactSlotsUI weaponSlots;

  [SerializeField]
  private ArtifactSlotsUI heartSlots;

  [SerializeField]
  private ArtifactSlotsUI accessorySlots;

  [Inject]
  private PlayerAccessor _playerAccessor;

  // Теперь мы обновляем слоты КАЖДЫЙ РАЗ при открытии интерфейса
  protected override void OnEnable()
  {
    base.OnEnable();
    RefreshSlots();
  }

  private void RefreshSlots()
  {
    var equipment = _playerAccessor.Equipment;

    // Сразу увидим в консоли, если проблема не в UI, а в том, что игрок потерялся
    if (equipment == null)
    {
      Debug.LogWarning(
        "[UI] Ошибка: PlayerEquipment = null. Игрок не зарегистрировался в PlayerAccessor!"
      );
      return;
    }

    if (equipment.Weapon != null)
      weaponSlots.Initialize(equipment.Weapon);
    if (equipment.Heart != null)
      heartSlots.Initialize(equipment.Heart);
    if (equipment.Accessory != null)
      accessorySlots.Initialize(equipment.Accessory);
  }
}
