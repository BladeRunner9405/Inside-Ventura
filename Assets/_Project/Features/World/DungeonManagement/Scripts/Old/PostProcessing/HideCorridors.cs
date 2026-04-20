using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideCorridorsPostProcess : DungeonGeneratorPostProcessingComponentGrid2D {
  public override void Run(DungeonGeneratorLevelGrid2D level) {
    Debug.Log("Hiding dungeon corridors...");
    foreach (var roomInstance in level.RoomInstances) {
      if (roomInstance.IsCorridor) {
        roomInstance.RoomTemplateInstance.SetActive(false);
      }
    }
    Debug.Log("Done hiding dungeon corridors");
  }
}
