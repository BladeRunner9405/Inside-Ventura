using Edgar.Unity;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPostProcessing : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField] private NavMeshSurface surface2D;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    AddFloorNavigation(level);
    AddWallsNavigation(level);

    // This is all needed because of some wacky bug.
    Invoke(nameof(BuildNavMesh), 0.5F);
  }

  private static void AddFloorNavigation(DungeonGeneratorLevelGrid2D level) {
    var floor = level.GetSharedTilemaps().Find(tilemap => tilemap.name == "Floor").gameObject;
    floor.AddComponent<NavMeshModifier>();
    floor.AddComponent<NavMeshModifierTilemap>();
  }

  private static void AddWallsNavigation(DungeonGeneratorLevelGrid2D level) {
    var walls = level.GetSharedTilemaps().Find(tilemap => tilemap.name == "Walls").gameObject;
    var wallsModifier = walls.AddComponent<NavMeshModifier>();
    walls.AddComponent<NavMeshModifierTilemap>();
    wallsModifier.overrideArea = true;
    var notWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
    wallsModifier.area = notWalkableArea;
  }

  private void BuildNavMesh() {
    surface2D.BuildNavMesh();
  }
}
