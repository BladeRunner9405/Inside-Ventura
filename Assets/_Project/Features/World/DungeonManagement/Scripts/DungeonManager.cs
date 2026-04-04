using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace InsideVentura.World
{
  public class DungeonManager : MonoBehaviour
  {
    public static DungeonManager Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    private RoomBuilder builder;

    [SerializeField]
    private EncounterManager encounterManager;

    [Header("Generation Settings")]
    [SerializeField]
    private int targetRoomCount = 8;

    [SerializeField]
    private DungeonRoomData safeRoomTemplate;

    [SerializeField]
    private List<DungeonRoomData> normalRoomTemplates;

    [SerializeField]
    private List<EncounterData> encounterPool;

    [Header("Player Settings")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private CinemachineCamera _playerCamera;

    private GameObject _activePlayer;
    private Dictionary<Vector2Int, RoomInstance> _dungeonMap =
      new Dictionary<Vector2Int, RoomInstance>();
    private Vector2Int _currentCoords = Vector2Int.zero;
    private List<Enemy> _enemiesInRoom = new List<Enemy>();

    private void Awake()
    {
      if (Instance == null)
        Instance = this;
      else
        Destroy(gameObject);
    }

    private void Start()
    {
      GenerateDungeon();
      LoadRoom(_currentCoords);

      CinemachineConfiner2D confiner = _playerCamera.GetComponent<CinemachineConfiner2D>();
      if (confiner != null)
        confiner.InvalidateBoundingShapeCache();

      InitialPlayerPlacement();
    }

    private void InitialPlayerPlacement()
    {
      if (playerPrefab != null && builder.PlayerStartPoint != null)
      {
        _activePlayer = Instantiate(
          playerPrefab,
          builder.PlayerStartPoint.position,
          Quaternion.identity
        );
        var entity = _activePlayer.GetComponent<Entity>();
        _playerCamera.Follow = entity != null ? entity.target : _activePlayer.transform;
      }
      else
      {
        Debug.LogError("[Dungeon] Не назначен префаб игрока или точка старта!");
      }
    }

    public void RegisterRoomObject(GameObject obj)
    {
      if (_dungeonMap.TryGetValue(_currentCoords, out RoomInstance room))
      {
        if (!room.RoomObjects.Contains(obj))
          room.RoomObjects.Add(obj);
      }
    }

    public bool CanExitRoom()
    {
      return encounterManager == null || !encounterManager.IsEncounterActive;
    }

    private void LoadRoom(Vector2Int coords)
    {
      RoomInstance instance = _dungeonMap[coords];

      bool hasNorth = _dungeonMap.ContainsKey(coords + Vector2Int.up);
      bool hasSouth = _dungeonMap.ContainsKey(coords + Vector2Int.down);
      bool hasEast = _dungeonMap.ContainsKey(coords + Vector2Int.right);
      bool hasWest = _dungeonMap.ContainsKey(coords + Vector2Int.left);

      builder.Build(instance.Data, instance.IsCleared, hasNorth, hasSouth, hasEast, hasWest);

      if (!instance.IsCleared && instance.Type != RoomType.Safe)
      {
        encounterManager.StartEncounter(
          instance.Encounter,
          builder.SpawnPoints,
          () =>
          {
            _dungeonMap[_currentCoords].IsCleared = true;
          }
        );
      }
    }

    public void MoveToRoom(DoorDirection doorDir)
    {
      // Скрываем объекты старой комнаты
      if (_dungeonMap.TryGetValue(_currentCoords, out RoomInstance oldRoom))
      {
        oldRoom.RoomObjects.RemoveAll(item => item == null);
        foreach (var obj in oldRoom.RoomObjects)
          obj.SetActive(false);
      }

      Vector2Int nextCoords = _currentCoords;
      switch (doorDir)
      {
        case DoorDirection.North:
          nextCoords += Vector2Int.up;
          break;
        case DoorDirection.South:
          nextCoords += Vector2Int.down;
          break;
        case DoorDirection.East:
          nextCoords += Vector2Int.right;
          break;
        case DoorDirection.West:
          nextCoords += Vector2Int.left;
          break;
      }

      if (_dungeonMap.ContainsKey(nextCoords))
      {
        _currentCoords = nextCoords;
        _enemiesInRoom.Clear();
        if (encounterManager != null)
          encounterManager.ClearCorpses();

        LoadRoom(_currentCoords);
        TeleportPlayer(doorDir);

        // Показываем объекты новой комнаты
        if (_dungeonMap.TryGetValue(_currentCoords, out RoomInstance newRoom))
        {
          newRoom.RoomObjects.RemoveAll(item => item == null);
          foreach (var obj in newRoom.RoomObjects)
            obj.SetActive(true);
        }
      }
    }

    private void TeleportPlayer(DoorDirection enteredDoor)
    {
      if (_activePlayer == null)
        return;

      DoorDirection targetDoorDir = GetOppositeDirection(enteredDoor);
      DungeonDoor[] newDoors = Object.FindObjectsByType<DungeonDoor>(FindObjectsSortMode.None);

      foreach (var door in newDoors)
      {
        if (door.direction == targetDoorDir)
        {
          Vector3 offset = Vector3.zero;
          float step = 1.5f;

          switch (targetDoorDir)
          {
            case DoorDirection.North:
              offset = Vector3.down * step;
              break;
            case DoorDirection.South:
              offset = Vector3.up * step;
              break;
            case DoorDirection.East:
              offset = Vector3.left * step;
              break;
            case DoorDirection.West:
              offset = Vector3.right * step;
              break;
          }

          _activePlayer.transform.position = door.transform.position + offset;
          return;
        }
      }
    }

    private void GenerateDungeon()
    {
      _dungeonMap.Clear();
      Vector2Int currentPos = Vector2Int.zero;
      _dungeonMap.Add(currentPos, new RoomInstance(safeRoomTemplate, RoomType.Safe, true));

      int roomsCreated = 1;
      while (roomsCreated < targetRoomCount)
      {
        Vector2Int dir = GetRandomDirectionVector();
        Vector2Int newPos = currentPos + dir;

        if (!_dungeonMap.ContainsKey(newPos))
        {
          var randomTemplate = normalRoomTemplates[Random.Range(0, normalRoomTemplates.Count)];
          EncounterData randomEncounter =
            encounterPool.Count > 0 ? encounterPool[Random.Range(0, encounterPool.Count)] : null;

          _dungeonMap.Add(
            newPos,
            new RoomInstance(randomTemplate, RoomType.Normal, false, randomEncounter)
          );
          roomsCreated++;
        }
        currentPos = newPos;
      }
    }

    public void GenerateNextLevel()
    {
      foreach (var room in _dungeonMap.Values)
      {
        foreach (var obj in room.RoomObjects)
          if (obj != null)
            Destroy(obj);
        room.RoomObjects.Clear();
      }

      if (encounterManager != null)
        encounterManager.ClearCorpses();
      _enemiesInRoom.Clear();
      _dungeonMap.Clear();
      _currentCoords = Vector2Int.zero;

      GenerateDungeon();
      LoadRoom(_currentCoords);

      if (_activePlayer != null && builder.PlayerStartPoint != null)
        _activePlayer.transform.position = builder.PlayerStartPoint.position;
    }

    private Vector2Int GetRandomDirectionVector()
    {
      int r = Random.Range(0, 4);
      return r switch
      {
        0 => Vector2Int.up,
        1 => Vector2Int.down,
        2 => Vector2Int.left,
        _ => Vector2Int.right,
      };
    }

    private DoorDirection GetOppositeDirection(DoorDirection dir)
    {
      return dir switch
      {
        DoorDirection.North => DoorDirection.South,
        DoorDirection.South => DoorDirection.North,
        DoorDirection.East => DoorDirection.West,
        DoorDirection.West => DoorDirection.East,
        _ => DoorDirection.South,
      };
    }
  }
}
