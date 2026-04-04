using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InsideVentura.World
{
  // 1. ОПРЕДЕЛЯЕМ ВСЕ ВОЗМОЖНЫЕ ТИПЫ ПОВЕДЕНИЯ КЛЕТКИ
  public enum CellBehavior
  {
    Empty, // Ничего не делать (только пол)
    Wall, // Поставить тайл в слой стен
    Prefab, // Заспавнить обычный префаб (декор, препятствие)
    Door, // Логика умной двери
    PlayerStart, // Сохранить координату для игрока
    RewardPoint, // Сохранить координату для сундука
    EnemySpawner, // Добавить в список точек спавна врагов

    NextLevel,
  }

  // 2. КЛАСС ДЛЯ ИНСПЕКТОРА
  [Serializable]
  public class CellMapping
  {
    public string note; // Для себя в инспекторе (например: "Стена", "Бочка")
    public int id; // Цифра из txt файла
    public CellBehavior behavior; // Как Строитель должен с этим работать

    [Header("Assets")]
    public TileBase tile; // Если это Wall
    public GameObject prefab; // Если это Prefab или Door
  }

  

  public class RoomBuilder : MonoBehaviour
  {
    [Header("Base Tilemaps")]
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private TileBase floorTile; // Пол везде одинаковый

    [SerializeField]
    private TileBase defaultWallTile; // Стена по умолчанию (для закрытых дверей)

    [Header("ID Configuration")]
    [SerializeField]
    private List<CellMapping> cellMappings = new List<CellMapping>();

    // Словарь для мгновенного поиска настроек по ID (создается при запуске)
    private Dictionary<int, CellMapping> _mapConfig = new Dictionary<int, CellMapping>();

    // --- Данные для менеджеров ---
    public Transform PlayerStartPoint { get; private set; }
    public Transform RewardPoint { get; private set; }
    public List<Transform> SpawnPoints { get; private set; } = new List<Transform>();

    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private DungeonRoomData _currentData;

    private List<DungeonDoor> _dungeonDoors = new List<DungeonDoor>();

    [Header("Camera Settings")]
    [SerializeField]
    private PolygonCollider2D cameraBoundsCollider;

    // 2. Добавь метод пересчета границ в конец класса:
    public void UpdateCameraBounds()
    {
      if (cameraBoundsCollider == null)
        return;

      // Сжимаем границы тайлмапа, чтобы получить точный размер пола
      floorTilemap.CompressBounds();
      Bounds bounds = floorTilemap.localBounds;

      // Создаем прямоугольник ровно по размеру тайлмапа пола
      Vector2[] points = new Vector2[4];
      points[0] = new Vector2(bounds.min.x, bounds.min.y);
      points[1] = new Vector2(bounds.min.x, bounds.max.y);
      points[2] = new Vector2(bounds.max.x, bounds.max.y);
      points[3] = new Vector2(bounds.max.x, bounds.min.y);

      cameraBoundsCollider.points = points;
    }

    private void Awake()
    {
      // Перегоняем список из инспектора в словарь для скорости
      foreach (var mapping in cellMappings)
      {
        if (!_mapConfig.ContainsKey(mapping.id))
        {
          _mapConfig.Add(mapping.id, mapping);
        }
      }
    }

    public void Build(
      DungeonRoomData data,
      bool isCleared,
      bool hasNorth,
      bool hasSouth,
      bool hasEast,
      bool hasWest
    )
    {
      if (data == null)
        return;
      _currentData = data;
      Clear();

      for (int y = 0; y < data.height; y++)
      {
        for (int x = 0; x < data.width; x++)
        {
          int id = data.GetID(x, y);
          Vector3Int tilePos = new Vector3Int(x, y, 0);

          // 1. Всегда ставим пол
          floorTilemap.SetTile(tilePos, floorTile);

          // 2. Ищем, что делать с этим ID
          if (!_mapConfig.TryGetValue(id, out CellMapping mapping))
            continue; // Если ID нет в настройках - пропускаем

          // 3. Обработка логики на основе ПОВЕДЕНИЯ (а не цифры!)
          switch (mapping.behavior)
          {
            case CellBehavior.Wall:
              wallTilemap.SetTile(tilePos, mapping.tile);
              break;

            case CellBehavior.Prefab:
              Spawn(mapping.prefab, tilePos);
              break;

            case CellBehavior.Door:
              DoorDirection dir = DetermineDoorDirection(x, y);
              bool keepDoor = dir switch
              {
                DoorDirection.North => hasNorth,
                DoorDirection.South => hasSouth,
                DoorDirection.East => hasEast,
                DoorDirection.West => hasWest,
                _ => false,
              };

              if (keepDoor)
              {
                SpawnDoor(mapping.prefab, tilePos, dir);
              }
              else
              {
                // Закрываем дверь глухой стеной
                wallTilemap.SetTile(tilePos, defaultWallTile);
              }
              break;

            case CellBehavior.PlayerStart:
              PlayerStartPoint = CreatePoint("PlayerStart", tilePos);
              break;

            case CellBehavior.RewardPoint:
              RewardPoint = CreatePoint("RewardPoint", tilePos);
              break;

            case CellBehavior.EnemySpawner:
              CreateSpawnPoint(tilePos);
              break;
            case CellBehavior.NextLevel:
              Spawn(mapping.prefab, tilePos);
              break;
          }
        }
      }
      if (isCleared) {
        foreach (DungeonDoor door in _dungeonDoors)
        {
          door.SetOpen();
        }
      } else {
        foreach (DungeonDoor door in _dungeonDoors)
        {
          door.SetClosed();
        }
      }
      UpdateCameraBounds();
    }

    // --- Вспомогательные методы остались без изменений ---

    private Transform CreatePoint(string name, Vector3Int tilePos)
    {
      GameObject obj = new GameObject(name);
      obj.transform.position = floorTilemap.GetCellCenterWorld(tilePos);
      obj.transform.SetParent(transform);
      _spawnedObjects.Add(obj);
      return obj.transform;
    }

    private DoorDirection DetermineDoorDirection(int x, int y)
    {
      if (y >= _currentData.height - 1)
        return DoorDirection.North;
      if (y <= 0)
        return DoorDirection.South;
      if (x >= _currentData.width - 1)
        return DoorDirection.East;
      return DoorDirection.West;
    }

    private void SpawnDoor(GameObject prefab, Vector3Int tilePos, DoorDirection dir)
    {
      if (prefab == null)
        return;
      Vector3 worldPos = floorTilemap.GetCellCenterWorld(tilePos);
      GameObject doorObj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
      _spawnedObjects.Add(doorObj);

      if (doorObj.TryGetComponent<DungeonDoor>(out var doorScript))
      {
        doorScript.direction = dir;
        _dungeonDoors.Add(doorScript);
      }
    }

    private void Spawn(GameObject prefab, Vector3Int tilePos)
    {
      if (prefab == null)
        return;
      Vector3 worldPos = floorTilemap.GetCellCenterWorld(tilePos);
      GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
      _spawnedObjects.Add(obj);
    }

    public void Clear()
    {
      PlayerStartPoint = null;
      RewardPoint = null;
      SpawnPoints.Clear();
      floorTilemap.ClearAllTiles();
      wallTilemap.ClearAllTiles();
      _dungeonDoors.Clear();

      foreach (var obj in _spawnedObjects)
      {
        if (obj != null)
          Destroy(obj);
      }
      _spawnedObjects.Clear();
    }

    private void CreateSpawnPoint(Vector3Int tilePos)
    {
      Vector3 worldPos = floorTilemap.GetCellCenterWorld(tilePos);
      GameObject spawnerObj = new GameObject($"SpawnPoint_{SpawnPoints.Count}");
      spawnerObj.transform.position = worldPos;
      spawnerObj.transform.SetParent(transform);
      SpawnPoints.Add(spawnerObj.transform);
      _spawnedObjects.Add(spawnerObj);
    }
  }
}
