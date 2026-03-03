using UnityEngine;


public class CurrentRoomDetectionTriggerHandler : MonoBehaviour {
  private CurrentRoomDetectionRoomManager roomManager;

  public void Start() {
    roomManager = transform.parent.parent.gameObject.GetComponent<CurrentRoomDetectionRoomManager>();
  }

  public void OnTriggerEnter2D(Collider2D otherCollider) {
    Debug.Log($"Something entered the room: {otherCollider.name}");
    if (otherCollider.gameObject.tag == "Player") {
      roomManager?.OnRoomEnter(otherCollider.gameObject);
    }
  }

  public void OnTriggerExit2D(Collider2D otherCollider) {
    if (otherCollider.gameObject.tag == "Player") {
      roomManager?.OnRoomLeave(otherCollider.gameObject);
    }
  }
}
