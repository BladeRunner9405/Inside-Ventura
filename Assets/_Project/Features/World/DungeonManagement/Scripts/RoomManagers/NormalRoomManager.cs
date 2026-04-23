using System.Collections.Generic;
using Edgar.Unity;
using InsideVentura.World.v1;
using UnityEngine;

namespace InsideVentura.World {
  public class NormalRoomManager : RoomManagerBase {
    private List<GameObject> _doors;

    private int _waveCount;
    private List<GameObject> _waves = new();
    private int _curWave = -1;

    private GameObject _chest;

    private int _remainingEnemiesCount;

    /// <summary>
    /// Whether enemies were spawned.
    /// </summary>
    private bool _enemiesSpawned;

    public override void Init(RoomInstanceGrid2D roomInstance) {
      base.Init(roomInstance);
      // _doors = DetectDoors(roomInstance);

      _chest = transform.Find("Chest")?.gameObject;
      if (_chest == null) {
        Debug.Log("No chest inside the room, are you sure?");
      }

      _chest?.SetActive(false);

      var wavesRoot = transform.Find("Waves")?.gameObject;
      if (wavesRoot == null) {
        Debug.Log("No Waves inside the room, are you sure?");
        _enemiesSpawned = true;
        return;
      }

      wavesRoot.SetActive(true);

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

      if (_enemiesSpawned) return;

      Debug.Log("Spawning enemies and closing all doors...");
      DoorsSetLocked(true);
      EncounterStatus.Active = true;
      AdvanceWave();
    }

    private void AdvanceWave() {
      _curWave++;
      if (_curWave == _waveCount) {
        SpawnChest();
        DoorsSetLocked(false);
        _enemiesSpawned = true;
        EncounterStatus.Active = false;
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

    private void DoorsSetLocked(bool locked) {
      foreach (var door in Doors) {
        if (locked) {
          door.SetClosed();
        }
        else {
          door.SetOpen();
        }
      }
    }

    private void SpawnChest() {
      _chest.SetActive(true);
    }
  }
}
