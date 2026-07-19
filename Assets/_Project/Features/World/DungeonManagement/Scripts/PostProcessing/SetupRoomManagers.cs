using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InsideVentura.World
{
  public class SetupRoomManagers : DungeonGeneratorPostProcessingComponentGrid2D
  {
    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
      level.GetSharedTilemaps().ForEach(x =>
      {
        if (x.gameObject.name == "Walls") x.gameObject.layer = 3;
      });

      Debug.Log("Setting up dungeon rooms...");

      var player = GameObject.FindGameObjectWithTag("Player");
      if (player == null)
      {
        Debug.Log("[SetupRoomManagers] Could not find player. The spawn OnRoomEnter is not triggered.");
        return;
      }

      foreach (var roomInstance in level.RoomInstances)
      {
        var roomTemplateInstance = roomInstance.RoomTemplateInstance;

        // Find floor tilemap layer
        var tilemaps = RoomTemplateUtilsGrid2D.GetTilemaps(roomTemplateInstance);
        var floor = tilemaps.Single(x => x.name == "Floor").gameObject;

        // Add floor collider
        AddFloorCollider(floor);

        // Add current room detection handler
        floor.AddComponent<RoomEnterTriggerHandler>();

        // Add the room manager component
        RoomManagerBase roomManager = AddRoomManager(roomTemplateInstance, roomInstance);
        roomManager.Init(roomInstance);

        var room = roomInstance.Room as DungeonRoom;
        if (room != null && room.type == DungeonRoomType.Spawn)
        {
          Debug.Log("Entering spawn room...");
          roomManager.OnRoomEnter(player);
        }
      }

      Debug.Log("Done setting up dungeon rooms");
    }

    private RoomManagerBase AddRoomManager(GameObject roomTemplateInstance, RoomInstanceGrid2D roomInstance)
    {
      var dungeonRoom = roomInstance.Room as DungeonRoom;
      switch (dungeonRoom.type)
      {
        case DungeonRoomType.NormalRank1 or DungeonRoomType.NormalRank2 or DungeonRoomType.NormalRank3:
          return roomTemplateInstance.AddComponent<NormalRoomManager>();
        default:
          return roomTemplateInstance.AddComponent<RoomManagerBase>();
      }
    }

    private void AddFloorCollider(GameObject floor)
    {
      var tilemapCollider2D = floor.AddComponent<TilemapCollider2D>();
      tilemapCollider2D.compositeOperation = Collider2D.CompositeOperation.Merge;


      var compositeCollider2D = floor.AddComponent<CompositeCollider2D>();
      compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
      compositeCollider2D.isTrigger = true;
      compositeCollider2D.generationType = CompositeCollider2D.GenerationType.Manual;

      floor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }
  }
}
