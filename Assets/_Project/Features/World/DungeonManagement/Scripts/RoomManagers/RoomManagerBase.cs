using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CherryFramework.BaseClasses;
using CherryFramework.DependencyManager;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;

namespace InsideVentura.World {
  public class RoomManagerBase : BehaviourBase {
    /// <summary>
    /// Room instance of the corresponding room.
    /// </summary>
    protected RoomInstanceGrid2D RoomInstance;

    protected List<DungeonDoor> Doors;

    [Inject] private DungeonManager _dungeonManager;
    private CompositeCollider2D _floorCollider;

    public DungeonRoom GetRoom()
    {
      var res = RoomInstance.Room as DungeonRoom;
      if (res == null)
      {
        Debug.LogError($"Room instance doesn't implement {nameof(DungeonRoom)}");
      }

      return res;
    }

    public virtual void Init(RoomInstanceGrid2D roomInstance) {
      RoomInstance = roomInstance;

      var floor = transform.Find("Tilemaps/Floor");
      _floorCollider = floor.GetComponent<CompositeCollider2D>();

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

      _floorCollider.GenerateGeometry();

      UpdateCameraBounds();
    }

    private void UpdateCameraBounds()
    {
      var cam = GameObject.Find("CinemachineCamera");
      var confiner = cam.GetComponent<CinemachineConfiner2D>();
      confiner.InvalidateBoundingShapeCache();
      confiner.BoundingShape2D = _floorCollider;
      confiner.BakeBoundingShape(cam.GetComponent<CinemachineCamera>(), 5);
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
