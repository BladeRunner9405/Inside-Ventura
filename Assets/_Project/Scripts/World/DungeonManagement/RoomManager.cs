using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.AI;

public class RoomManager : MonoBehaviour {
  /// <summary>
  /// Enemies that can spawn inside the room.
  /// </summary>
  private GameObject[] enemyPrefabs;

  private int remainingEnemiesCount;

  private List<GameObject> doors;

  /// <summary>
  /// Whether enemies were spawned.
  /// </summary>
  private bool enemiesSpawned;

  /// <summary>
  /// Collider of the floor tilemap layer.
  /// </summary>
  private Collider2D floorCollider;


  /// <summary>
  /// Room instance of the corresponding room.
  /// </summary>
  private RoomInstanceGrid2D roomInstance;

  public void Init(GameObject[] enemyPrefabs, Collider2D floorCollider,
    RoomInstanceGrid2D roomInstance) {
    this.enemyPrefabs = enemyPrefabs;
    this.floorCollider = floorCollider;
    this.roomInstance = roomInstance;

    doors = new();
    foreach (var door in roomInstance.Doors) {
      // Get the room instance of the room that is connected via this door
      var corridorRoom = door.ConnectedRoomInstance;

      // Get the room template instance of the corridor room
      var corridorGameObject = corridorRoom.RoomTemplateInstance;

      // Find the door game object by its name
      var doorsGameObject = corridorGameObject.transform.Find("Door")?.gameObject;

      if (doorsGameObject != null) {
        doorsGameObject.SetActive(false);
        doors.Add(doorsGameObject);
      }
      else {
        Debug.Log("No doors in the corridor, are you sure?");
      }
    }

    // Otherwise the enemy gets teleported after being spawned.
    foreach (var enemy in enemyPrefabs) {
      enemy.GetComponent<NavMeshAgent>().enabled = false;
    }
  }

  /// <summary>
  /// Use the shared Random instance so that the results are properly seeded.
  /// </summary>
  private static System.Random Random => DungeonManager.Instance.Random;

  /// <summary>
  /// Gets called when a player enters the room.
  /// </summary>
  /// <param name="player"></param>
  public void OnRoomEnter(GameObject player) {
    if (roomInstance == null) return;

    Debug.Log(
      $"Room enter. Room name: {roomInstance.Room.GetDisplayName()}, Room template: {roomInstance.RoomTemplatePrefab.name}");

    var type = ((DungeonRoom)roomInstance.Room).type;
    if (roomInstance.IsCorridor || type != DungeonRoomType.Normal || enemiesSpawned) return;

    Debug.Log("Spawning enemies and closing all doors...");
    SpawnEnemies();
    foreach (var door in doors) {
      door.SetActive(true);
    }
  }

  /// <summary>
  /// Gets called when a player leaves the room.
  /// </summary>
  /// <param name="player"></param>
  public void OnRoomLeave(GameObject player) {
    if (roomInstance != null) {
      Debug.Log($"Room leave {roomInstance.Room.GetDisplayName()}");
    }
  }


  private void SpawnEnemies() {
    enemiesSpawned = true;

    var enemies = new List<Enemy>();
    var totalEnemiesCount = Random.Next(4, 8);
    Debug.Log(floorCollider.bounds);
    while (enemies.Count < totalEnemiesCount) {
      // Find random position inside floor collider bounds
      var position = RandomPointInBounds(floorCollider.bounds, 1f);

      // Check if the point is actually inside the collider as there may be holes in the floor, etc.
      if (!IsPointWithinCollider(floorCollider, position)) {
        continue;
      }

      // We want to make sure that there is no other collider in the radius of 1
      if (Physics2D.OverlapCircleAll(position, 0.5f).Any(x => !x.isTrigger)) {
        continue;
      }

      // Pick random enemy prefab
      var enemyPrefab = enemyPrefabs[Random.Next(0, enemyPrefabs.Length)];

      // Create an instance of the enemy and set position and parent
      var enemy = Instantiate(enemyPrefab, roomInstance.RoomTemplateInstance.transform, true);
      enemy.transform.position = position;
      Debug.Log(position);

      var gungeonEnemy = enemy.GetComponent<Enemy>();
      gungeonEnemy.OnDeath += () => {
        Debug.Log($"Enemy died, remaining enemies: {--remainingEnemiesCount}");

        if (remainingEnemiesCount != 0) return;

        foreach (var door in doors) {
          door.SetActive(false);
        }
      };
      enemies.Add(gungeonEnemy);
    }

    remainingEnemiesCount = totalEnemiesCount;
  }

  private static bool IsPointWithinCollider(Collider2D collider, Vector2 point) {
    return collider.OverlapPoint(point);
  }

  private static Vector3 RandomPointInBounds(Bounds bounds, float margin = 0) {
    return new Vector3(
      RandomRange(bounds.min.x + margin, bounds.max.x - margin),
      RandomRange(bounds.min.y + margin, bounds.max.y - margin),
      // RandomRange(bounds.min.z + margin, bounds.max.z - margin)
      0
    );
  }

  private static float RandomRange(float min, float max) {
    return (float)(Random.NextDouble() * (max - min) + min);
  }
}
