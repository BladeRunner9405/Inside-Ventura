using Edgar.Unity;
using UnityEngine;

namespace InsideVentura.World {
  public class DetachPlayerPostProcess : DungeonGeneratorPostProcessingComponentGrid2D {
    private DungeonGeneratorLevelGrid2D _level;

    public override void Run(DungeonGeneratorLevelGrid2D level) {
      _level = level;
      Invoke(nameof(DetachPlayer), 0.1f);
    }

    private void DetachPlayer() {
      Debug.Log("Detaching player from spawn room...");

      var player = GameObject.FindGameObjectWithTag("Player");
      if (player == null) {
        Debug.Log("[DetachPlayerPostprocess] Player not found");
        return;
      }

      player.transform.SetParent(_level.RootGameObject.transform, true);

      Debug.Log("Done detaching player from spawn room");
    }
  }

}
