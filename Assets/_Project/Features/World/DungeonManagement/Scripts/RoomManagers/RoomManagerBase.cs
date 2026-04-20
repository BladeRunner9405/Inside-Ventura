using System.Collections.Generic;
using System.Linq;
using CherryFramework.BaseClasses;
using CherryFramework.DependencyManager;
using Edgar.Unity;
using UnityEngine;

namespace InsideVentura.World {
  public class RoomManagerBase : BehaviourBase {
    /// <summary>
    /// Room instance of the corresponding room.
    /// </summary>
    protected RoomInstanceGrid2D RoomInstance;

    protected List<DungeonDoor> Doors;

    [Inject] private DungeonManagerOld _dungeonManager;

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
      _dungeonManager.CurrentRoom = this;
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
}
