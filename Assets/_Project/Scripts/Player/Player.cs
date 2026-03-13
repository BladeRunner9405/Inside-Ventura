using CherryFramework.DependencyManager;
using UnityEngine;

public class Player : Entity {
  [SerializeField] private ItemPickup itemPickup;

  [SerializeField] private PlayerInventory inventory;

  [Inject] private PlayerAccessor _playerAccessor;
  public PlayerInventory Inventory => inventory;

  protected override void OnEnable() {
    base.OnEnable();
    _playerAccessor.RegisterPlayer(this);
  }

  private void OnDisable() {
    _playerAccessor.UnregisterPlayer(this);
  }

  public void TryToInteract() {
    itemPickup.TryToInteract();
  }

  /*void Heal(int amount) {
      if (IsDead) return;
      if (amount <= 0) return;

      Health += amount;
  }

  void ModifyMaxHealth(int delta) {
    if (IsDead) return;

    MaxHealth += delta;
  }*/
}
