using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideAllButSpawnPostProcess : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField, Tooltip("Mark this if you don't want to see all level in the editor")]
  private bool runInEditor = false;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    Debug.Log("Hiding every room except spawn room...");
    if ((Application.isEditor && !Application.isPlaying) && !runInEditor) {
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
