using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideAllButSpawnPostProcess : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField, Tooltip("Run this post process? Unmark if you want to get a look over the generated level.")]
  private bool run = false;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    Debug.Log("Hiding every room except spawn room...");
    if (!run) {
      Debug.Log("Skipping, the flag run is not set");
      return;
    }


    foreach (var roomInstance in level.RoomInstances) {
      var dungeonRoom = roomInstance.Room as DungeonRoom;
      if (dungeonRoom == null) {
        Debug.LogWarning("DungeonRoom instance is not of a DungeonRoom type");
        continue;
      }

      if (dungeonRoom.type == DungeonRoomType.Spawn) {
        continue;
      }

      roomInstance.RoomTemplateInstance.SetActive(false);
    }

    Debug.Log("Done hiding every rooms");
  }
}
