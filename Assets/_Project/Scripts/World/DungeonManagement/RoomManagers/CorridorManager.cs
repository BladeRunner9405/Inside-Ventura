using Edgar.Unity;
using UnityEngine;

public class CorridorManager : RoomManagerBase {
  // Does nothing, because this is corridor.
  public override void Init(Collider2D floorCollider, RoomInstanceGrid2D roomInstance) {
    base.Init(floorCollider, roomInstance);
  }

  public override void OnRoomEnter(GameObject player) {
    base.OnRoomEnter(player);
  }

  public override void OnRoomLeave(GameObject player) {
    base.OnRoomLeave(player);
  }
}
