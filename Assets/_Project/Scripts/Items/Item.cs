using UnityEngine;

public abstract class Item : InteractableObject {
  public virtual void OnPickup(GameObject player) {
  }

  public override void Interact(Player player) {
    base.Interact(player);
  }
  // private void OnTriggerEnter2D(Collider2D other) {
  //   if (other.CompareTag("Player")) {
  //     OnPickup(other.gameObject);
  //     // А может и не так, надо подумать
  //   }
  // }
}
