using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SetupRoomDoors : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField] private DungeonDoor doorPrefab;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    Debug.Log("Setting up dungeon doors...");

    foreach (var roomInstance in level.RoomInstances) {
      foreach (var doorInstance in roomInstance.Doors) {
        var worldPos = doorInstance.DoorLine.From + roomInstance.Position;
        Instantiate(doorPrefab, worldPos, Quaternion.identity, roomInstance.RoomTemplateInstance.transform);
      }
    }

    Debug.Log("Done setting up dungeon rooms");
  }
}
