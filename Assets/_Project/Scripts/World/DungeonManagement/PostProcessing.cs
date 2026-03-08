using System.Linq;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationPostProcessing : DungeonGeneratorPostProcessingComponentGrid2D {
  [SerializeField]
  private GameObject[] enemies;

  public override void Run(DungeonGeneratorLevelGrid2D level) {
    level.GetSharedTilemaps().ForEach(x => {
      if (x.gameObject.name == "Walls") x.gameObject.layer = 3;
    });

    Debug.Log("Setting up dungeon rooms...");
    foreach (var roomInstance in level.RoomInstances) {
      var roomTemplateInstance = roomInstance.RoomTemplateInstance;

      // Find floor tilemap layer
      var tilemaps = RoomTemplateUtilsGrid2D.GetTilemaps(roomTemplateInstance);
      var floor = tilemaps.Single(x => x.name == "Floor").gameObject;

      // Add floor collider
      AddFloorCollider(floor);

      // Add current room detection handler
      floor.AddComponent<RoomEnterTriggerHandler>();

      // Add the room manager component
      var roomManager = roomTemplateInstance.AddComponent<RoomManager>();
      roomManager.Init(enemies, floor.GetComponent<CompositeCollider2D>(), roomInstance);
    }

    Debug.Log("Done setting up dungeon rooms");
  }

  private void AddFloorCollider(GameObject floor) {
    var tilemapCollider2D = floor.AddComponent<TilemapCollider2D>();
    tilemapCollider2D.compositeOperation = Collider2D.CompositeOperation.Merge;


    var compositeCollider2D = floor.AddComponent<CompositeCollider2D>();
    compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
    compositeCollider2D.isTrigger = true;
    compositeCollider2D.generationType = CompositeCollider2D.GenerationType.Manual;

    floor.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
  }
}
