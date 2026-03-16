using UnityEngine;

public class ThoughtItem : Item {
  [SerializeField] private Thought thoughtData;

  protected override bool CanPickUp() {
    if (!PlayerAccessor.Player?.Inventory) return false;

    return PlayerAccessor.Player.Inventory.CanAddThought();
  }

  protected override void OnPickup() {
    if (!PlayerAccessor.Player?.Inventory) return;

    PlayerAccessor.Player.Inventory.AddThoughtToBag(thoughtData);
  }
}
