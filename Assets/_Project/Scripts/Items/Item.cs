using UnityEngine;

public abstract class Item : MonoBehaviour {
  public virtual void OnPickup(GameObject player) {
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Player")) {
      OnPickup(other.gameObject);
      // А может и не так, надо подумать
    }
  }
}
