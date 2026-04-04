using System.Collections.Generic;
using System.Linq; // Добавлено для удобства поиска
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

    // НОВОЕ: Шаблон и энкаунтер для босса
    [SerializeField]
    private DungeonRoomData bossRoomTemplate;

    [SerializeField]
    private EncounterData bossEncounter;

    [SerializeField]
    private List<EncounterData> encounterPool;

    [Header("Player Settings")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private CinemachineCamera _playerCamera;
    private CinemachineConfiner2D _confiner;

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
      _confiner = _playerCamera.GetComponent<CinemachineConfiner2D>();
      if (_confiner != null)
        _confiner.InvalidateBoundingShapeCache();

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
            _dungeonMap[coords].IsCleared = true; // Исправлено: используем coords из параметров
          }
        );
      }
    }

    public void MoveToRoom(DoorDirection doorDir)
    {
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

          // Запоминаем позицию ДО телепортации для вычисления дельты
          Vector3 oldPos = _activePlayer.transform.position;
          Vector3 newPos = door.transform.position + offset;

          // 1. Телепортируем игрока
          _activePlayer.transform.position = newPos;

          // 2. Телепортируем камеру (OnTargetObjectWarped)
          // Параметры: (Трансформ цели, вектор смещения)
          _playerCamera.OnTargetObjectWarped(_activePlayer.transform, newPos - oldPos);

          if (_confiner != null)
            _confiner.InvalidateBoundingShapeCache();

          return;
        }
      }
    }

    private void GenerateDungeon()
    {
      _dungeonMap.Clear();
      Vector2Int currentPos = Vector2Int.zero;

      // 1. Создаем стартовую комнату
      _dungeonMap.Add(currentPos, new RoomInstance(safeRoomTemplate, RoomType.Safe, true));

      int roomsCreated = 1;

      // 2. Генерируем обычные комнаты
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

      // 3. ФИНАЛИЗАЦИЯ: Ищем самую дальнюю комнату для БОССА
      Vector2Int bossCoords = Vector2Int.zero;
      float maxDistance = -1f;

      foreach (var kvp in _dungeonMap)
      {
        // Считаем расстояние от старта (0,0)
        float dist = kvp.Key.sqrMagnitude;
        if (dist > maxDistance)
        {
          maxDistance = dist;
          bossCoords = kvp.Key;
        }
      }

      // 4. Заменяем самую дальнюю комнату на комнату босса
      if (bossCoords != Vector2Int.zero)
      {
        // Если Boss Room Template не задан, используем обычный, но меняем тип
        var template = bossRoomTemplate != null ? bossRoomTemplate : _dungeonMap[bossCoords].Data;

        _dungeonMap[bossCoords] = new RoomInstance(template, RoomType.Boss, false, bossEncounter);

        Debug.Log(
          $"<color=red>[Dungeon]</color> Комната Босса создана на координатах {bossCoords}"
        );
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

      if (_confiner != null)
        _confiner.InvalidateBoundingShapeCache();
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
