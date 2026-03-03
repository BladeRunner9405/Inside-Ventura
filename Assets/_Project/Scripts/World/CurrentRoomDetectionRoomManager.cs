using System.Collections.Generic;
using Edgar.Unity;
using UnityEngine;

public class CurrentRoomDetectionRoomManager : MonoBehaviour {
    /// <summary>
    /// Enemies that can spawn inside the room.
    /// </summary>
    public GameObject[] EnemyPrefabs;

    /// <summary>
    /// Enemies that are still alive in the room. (Do not change manually)
    /// </summary>
    public List<GungeonEnemy> RemainingEnemies;

    /// <summary>
    /// Whether enemies were spawned.
    /// </summary>
    public bool EnemiesSpawned;

    /// <summary>
    /// Collider of the floor tilemap layer.
    /// </summary>
    public Collider2D FloorCollider;


  /// <summary>
  /// Room instance of the corresponding room.
  /// </summary>
  public RoomInstanceGrid2D RoomInstance;

  /// <summary>
  /// Gets called when a player enters the room.
  /// </summary>
  /// <param name="player"></param>
  public void OnRoomEnter(GameObject player) {
    Debug.Log($"Room enter. Room name: {RoomInstance.Room.GetDisplayName()}, Room template: {RoomInstance.RoomTemplatePrefab.name}");
    CurrentRoomDetectionGameManager.Instance.OnRoomEnter(RoomInstance);
  }

  /// <summary>
  /// Gets called when a player leaves the room.
  /// </summary>
  /// <param name="player"></param>
  public void OnRoomLeave(GameObject player) {
    Debug.Log($"Room leave {RoomInstance.Room.GetDisplayName()}");
    if (CurrentRoomDetectionGameManager.Instance) {
      CurrentRoomDetectionGameManager.Instance.OnRoomLeave(RoomInstance);
    }
  }
}

