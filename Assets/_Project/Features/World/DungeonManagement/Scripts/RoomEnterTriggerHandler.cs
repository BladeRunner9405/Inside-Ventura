using UnityEngine;

public class RoomEnterTriggerHandler : MonoBehaviour {
  private RoomManagerBase _roomManagerBase;

  public void Start() {
    _roomManagerBase = transform.parent.parent.gameObject.GetComponent<RoomManagerBase>();
  }

  public void OnTriggerEnter2D(Collider2D otherCollider) {
    if (otherCollider.gameObject.CompareTag("Player")) {
      _roomManagerBase?.OnRoomEnter(otherCollider.gameObject);
    }
  }

  public void OnTriggerExit2D(Collider2D otherCollider) {
    if (otherCollider.gameObject.CompareTag("Player")) {
      _roomManagerBase?.OnRoomLeave(otherCollider.gameObject);
    }
  }
}
