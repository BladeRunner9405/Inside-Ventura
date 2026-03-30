using UnityEngine;

public class ThoughtItem : Item {
  [SerializeField] private Thought thoughtData;

  protected override bool CanPickUp() {
    if (!PlayerAccessor.Inventory) return false;

    return PlayerAccessor.Inventory.CanAddThought();
  }

  protected override void OnPickup() {
    if (!PlayerAccessor.Inventory) return;

    PlayerAccessor.Inventory.AddThoughtToBag(thoughtData);
  }
}
