using System.Linq;
using Edgar.Unity;
using Unity.Cinemachine;
using UnityEngine;

public class AdjustCameraPostProcessing : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField] private CinemachineCamera playerCamera;

  private DungeonGeneratorLevelGrid2D _level;
  public override void Run(DungeonGeneratorLevelGrid2D level) {
    _level = level;
    Invoke(nameof(Adjust), 0.1f);
  }

  private void Adjust() {
    Debug.Log("[AdjustCamera] Updating CinemachineCamera Follow");
    if (playerCamera == null) {
      Debug.LogError("[AdjustCamera] Error: Player Camera is not set in inspector!");
      return;
    }

    var player = GameObject.FindGameObjectWithTag("Player");
    if (player == null) {
      Debug.LogError("[AdjustCamera] Error: no object with the tag Player found");
      return;
    }

    var entity = player.GetComponent<Entity>();
    playerCamera.Follow = entity != null ? entity.target : player.transform;

    var confiner = playerCamera.GetComponent<CinemachineConfiner2D>();
    if (confiner == null) {
      Debug.Log("[AdjustCamera] No confiner found on Player Camera object");
      return;
    }

    var sharedWalls = _level.GetSharedTilemaps().Single(x => x.name == "Walls");
    var wallsCollider = sharedWalls.GetComponent<CompositeCollider2D>();
    wallsCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
    confiner.BoundingShape2D = wallsCollider;
  }
}
