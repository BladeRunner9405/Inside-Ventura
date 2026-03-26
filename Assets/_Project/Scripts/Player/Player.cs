using CherryFramework.DependencyManager;
using UnityEngine;

public class Player : Entity {
  [Header("Player stuff")] [SerializeField]
  private ItemPickup itemPickup;

  [SerializeField] private PlayerInventory inventory;
  [SerializeField] private PlayerEquipment equipment;
  [SerializeField] private PlayerStats stats;

  [Inject] private PlayerAccessor _playerAccessor;

  public PlayerInventory Inventory => inventory;
  public PlayerEquipment Equipment => equipment;
  public PlayerStats Stats => stats;

  protected override void OnEnable() {
    base.OnEnable();
    _playerAccessor.RegisterPlayer(this);

    OnTakeDamage += TriggerCameraShake;
  }

  private void OnDisable() {
    _playerAccessor.UnregisterPlayer(this);

    OnTakeDamage -= TriggerCameraShake;
  }

  public void TryToInteract() {
    itemPickup.TryToInteract();
  }

  private void TriggerCameraShake(float damageAmount)
  {
      // Интенсивность (например, 3f) и время (например, 0.2 секунды)
      // Можно даже привязать силу тряски к размеру полученного урона!
      CameraShaker.Instance.ShakeCamera(damageAmount);
  }
}
