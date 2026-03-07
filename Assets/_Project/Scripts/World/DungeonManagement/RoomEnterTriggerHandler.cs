using UnityEngine;

public class RoomEnterTriggerHandler : MonoBehaviour {
  private RoomManager roomManager;

  public void Start() {
    roomManager = transform.parent.parent.gameObject.GetComponent<RoomManager>();
  }

  public void OnTriggerEnter2D(Collider2D otherCollider) {
    if (otherCollider.gameObject.CompareTag("Player")) {
      roomManager?.OnRoomEnter(otherCollider.gameObject);
    }
  }

  public void OnTriggerExit2D(Collider2D otherCollider) {
    if (otherCollider.gameObject.CompareTag("Player")) {
      roomManager?.OnRoomLeave(otherCollider.gameObject);
    }
  }
}
