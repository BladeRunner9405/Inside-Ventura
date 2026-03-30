using CherryFramework.DependencyManager;
using CherryFramework.UI.InteractiveElements.Presenters;
using UnityEngine;

public class ThoughtEquipmentPresenter : PresenterBase
{
  [SerializeField] private ArtifactSlotsUI weaponSlots;
  [SerializeField] private ArtifactSlotsUI heartSlots;
  [SerializeField] private ArtifactSlotsUI accessorySlots;

  [Inject] private PlayerAccessor _playerAccessor;
  private PlayerEquipment playerEquipment;

  protected override void OnPresenterInitialized()
  {
    base.OnPresenterInitialized();
    playerEquipment = _playerAccessor.Equipment;

    if (weaponSlots != null && playerEquipment.Weapon != null)
      weaponSlots.Initialize(playerEquipment.Weapon);
    if (heartSlots != null && playerEquipment.Heart != null)
      heartSlots.Initialize(playerEquipment.Heart);
    if (accessorySlots != null && playerEquipment.Accessory != null)
      accessorySlots.Initialize(playerEquipment.Accessory);
  }
}
