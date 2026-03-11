using UnityEngine;

public class ThoughtItem : Item {
  [SerializeField] private Thought thoughtData;

  protected override void OnPickup() {
    if (!PlayerAccessor.Player?.Inventory) return;

    PlayerAccessor.Player.Inventory.AddThoughtToBag(thoughtData);
  }
}
