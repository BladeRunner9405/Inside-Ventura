using Edgar.Unity;
using UnityEngine;

public class SetupRoomDoors : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField] private DungeonDoor doorPrefab;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    Debug.Log("Setting up custom dungeon doors...");

    foreach (var roomInstance in level.RoomInstances) {
      foreach (var doorInstance in roomInstance.Doors) {
        var worldPos = doorInstance.DoorLine.From + roomInstance.Position;
        var spawnedDoor = Instantiate(doorPrefab, worldPos, Quaternion.identity,
          roomInstance.RoomTemplateInstance.transform);
        spawnedDoor.Init(doorInstance);
        spawnedDoor.SetOpen();
      }
    }

    Debug.Log("Done setting up dungeon rooms");
  }
}
