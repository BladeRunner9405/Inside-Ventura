using System;
using CherryFramework.DependencyManager;
using UnityEngine;

[DefaultExecutionOrder(1)] // это чтобы Instances инициализировались позже PlayerDataModel.
                           // В частности, HeartInstance меняет статы оттуда
public class PlayerEquipment : InjectMonoBehaviour
{
  [Header("Static Data (Scriptable Objects)")]
  [SerializeField]
  private Weapon weaponData;

  [SerializeField]
  private Heart heartData;

  [SerializeField]
  private Accessory accessoryData;

  [Inject]
  private PlayerAccessor _playerAccessor;

  public WeaponInstance Weapon { get; private set; }
  public HeartInstance Heart { get; private set; }
  public AccessoryInstance Accessory { get; private set; }

  public event Action OnInstancesInitialized;
  public event Action<ArtifactInstance, int, Thought> OnThoughtEquipped;
  public event Action<ArtifactInstance, int> OnThoughtUnequipped;

  protected override void OnEnable()
  {
    base.OnEnable(); // Заполняем _playerAccessor через [Inject]

    // Создаем инстансы СРАЗУ ЖЕ, чтобы UI не прочитал пустоту
    InitializeInstances();
  }

  private void InitializeInstances()
  {
    // Проверяем на null, чтобы не создавать дважды при переключении GameObject
    if (Weapon == null && weaponData != null)
    {
      Weapon = new WeaponInstance(weaponData, _playerAccessor);
      Weapon.OnThoughtEquipped += (slot, thought) =>
        OnThoughtEquipped?.Invoke(Weapon, slot, thought);
      Weapon.OnThoughtUnequipped += (slot) => OnThoughtUnequipped?.Invoke(Weapon, slot);
    }

    if (Heart == null && heartData != null)
    {
      Heart = new HeartInstance(heartData, _playerAccessor);
      Heart.OnThoughtEquipped += (slot, thought) => OnThoughtEquipped?.Invoke(Heart, slot, thought);
      Heart.OnThoughtUnequipped += (slot) => OnThoughtUnequipped?.Invoke(Heart, slot);
    }

    if (Accessory == null && accessoryData != null)
    {
      Accessory = new AccessoryInstance(accessoryData, _playerAccessor);
      Accessory.OnThoughtEquipped += (slot, thought) =>
        OnThoughtEquipped?.Invoke(Accessory, slot, thought);
      Accessory.OnThoughtUnequipped += (slot) => OnThoughtUnequipped?.Invoke(Accessory, slot);
    }

    OnInstancesInitialized?.Invoke();
  }

  public void EquipThoughtToWeapon(Thought thought, int slotIndex) =>
    Weapon?.EquipThought(thought, slotIndex);

  public void EquipThoughtToHeart(Thought thought, int slotIndex) =>
    Heart?.EquipThought(thought, slotIndex);

  public void EquipThoughtToAccessory(Thought thought, int slotIndex) =>
    Accessory?.EquipThought(thought, slotIndex);

  public void TryToAttack(Vector2 direction) => Weapon?.TryAttack(direction);

  public void TryToUseAbility(Vector2 direction) => Accessory?.TryUseAbility(direction);
}
