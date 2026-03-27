using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using UnityEngine;

public class NormalRoomManager : RoomManagerBase {
  private List<GameObject> _doors;

  private int _waveCount;
  private List<GameObject> _waves = new List<GameObject>();
  private int _curWave = -1;

  private GameObject _chest;

  /// <summary>
  /// Collider of the floor tilemap layer.
  /// </summary>
  private Collider2D _floorCollider;

  /// <summary>
  /// Enemies that can spawn inside the room.
  /// </summary>
  private GameObject[] _enemyPrefabs;

  private int _remainingEnemiesCount;

  /// <summary>
  /// Whether enemies were spawned.
  /// </summary>
  private bool _enemiesSpawned;

  public override void Init(Collider2D floorCollider, RoomInstanceGrid2D roomInstance) {
    base.Init(floorCollider, roomInstance);

    _floorCollider = floorCollider;
    _doors = DetectDoors(roomInstance);

    _chest = transform.Find("Chest").gameObject;
    if (_chest == null) {
      Debug.Log("No chest inside the room, are you sure?");
    }
    _chest.SetActive(false);

    var wavesRoot = transform.Find("Waves").gameObject;
    wavesRoot.SetActive(true);
    if (wavesRoot == null) {
      Debug.Log("No Waves inside the room, are you sure?");
    }

    // Detect waves.
    while (true) {
      var newWave = wavesRoot.transform.Find((_waveCount + 1).ToString());

      if (newWave == null) {
        break;
      }

      _waveCount++;
      newWave.gameObject.SetActive(false);
      _waves.Add(newWave.gameObject);
    }
  }

  private static List<GameObject> DetectDoors(RoomInstanceGrid2D roomInstance) {
    List<GameObject> res = new List<GameObject>();
    foreach (var door in roomInstance.Doors) {
      // Get the room instance of the room that is connected via this door
      var corridorRoom = door.ConnectedRoomInstance;

      // Get the room template instance of the corridor room
      var corridorGameObject = corridorRoom.RoomTemplateInstance;

      // Find the door game object by its name
      var doorsGameObject = corridorGameObject.transform.Find("Door")?.gameObject;

      if (doorsGameObject != null) {
        doorsGameObject.SetActive(false);
        res.Add(doorsGameObject);
      }
      else {
        Debug.Log("No doors in the corridor, are you sure?");
      }
    }

    return res;
  }

  public override void OnRoomEnter(GameObject player) {
    if (RoomInstance == null) return;
    base.OnRoomEnter(player);

    Debug.Log("Spawning enemies and closing all doors...");
    foreach (var door in _doors) {
      door.SetActive(true);
    }

    AdvanceWave();
  }

  private void AdvanceWave() {
    _curWave++;
    if (_curWave == _waveCount) {
      SpawnChest();
      foreach (var door in _doors) {
        door.SetActive(false);
      }
      return;
    }

    var nextWave = _waves[_curWave];
    nextWave.SetActive(true);

    var enemies = nextWave.GetComponentsInChildren<Enemy>();
    _remainingEnemiesCount = enemies.Length;
    foreach (var enemy in enemies) {
      enemy.gameObject.SetActive(true);
      enemy.OnDeath += () => {
        _remainingEnemiesCount--;
        if (_remainingEnemiesCount == 0) {
          AdvanceWave();
        }
      };
    }
  }

  private void SpawnChest() {
    _chest.SetActive(true);
  }

  private void SpawnEnemies() {
    _enemiesSpawned = true;

    var enemies = new List<Enemy>();
    var totalEnemiesCount = Random.Next(1, 3);
    Debug.Log(_floorCollider.bounds);
    while (enemies.Count < totalEnemiesCount) {
      // Find random position inside floor collider bounds
      var position = RandomPointInBounds(_floorCollider.bounds, 1f);

      // Check if the point is actually inside the collider as there may be holes in the floor, etc.
      if (!IsPointWithinCollider(_floorCollider, position)) {
        continue;
      }

      // We want to make sure that there is no other collider in the radius of 1
      if (Physics2D.OverlapCircleAll(position, 0.5f).Any(x => !x.isTrigger)) {
        continue;
      }

      // Pick random enemy prefab
      var enemyPrefab = _enemyPrefabs[Random.Next(0, _enemyPrefabs.Length)];

      // Create an instance of the enemy and set position and parent
      var enemy = Instantiate(enemyPrefab, RoomInstance.RoomTemplateInstance.transform, true);
      enemy.transform.position = position;
      Debug.Log(position);

      var gungeonEnemy = enemy.GetComponent<Enemy>();
      gungeonEnemy.OnDeath += () => {
        Debug.Log($"Enemy died, remaining enemies: {--_remainingEnemiesCount}");

        if (_remainingEnemiesCount != 0) return;

        foreach (var door in _doors) {
          door.SetActive(false);
        }
      };
      enemies.Add(gungeonEnemy);
    }

    _remainingEnemiesCount = totalEnemiesCount;
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
