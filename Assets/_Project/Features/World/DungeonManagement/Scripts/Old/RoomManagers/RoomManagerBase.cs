using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.AI;

public class RoomManagerBase : MonoBehaviour {
  /// <summary>
  /// Room instance of the corresponding room.
  /// </summary>
  protected RoomInstanceGrid2D RoomInstance;

  protected List<DungeonDoor> Doors;

  public virtual void Init(RoomInstanceGrid2D roomInstance) {
    RoomInstance = roomInstance;

    Doors = GetComponentsInChildren<DungeonDoor>().ToList();
    foreach (var door in Doors) {
      door.SetOpen();
    }
  }

  /// <summary>
  /// Gets called when a player enters the room.
  /// </summary>
  /// <param name="player"></param>
  public virtual void OnRoomEnter(GameObject player) {
    if (RoomInstance == null) return;

    Debug.Log(
      $"Room enter. Room name: {RoomInstance.Room.GetDisplayName()}, Room template: {RoomInstance.RoomTemplatePrefab.name}");
  }

  /// <summary>
  /// Gets called when a player leaves the room.
  /// </summary>
  /// <param name="player"></param>
  public virtual void OnRoomLeave(GameObject player) {
    if (RoomInstance != null) {
      Debug.Log($"Room leave {RoomInstance.Room.GetDisplayName()}");
    }
  }
}
