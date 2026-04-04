using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InsideVentura.World
{
  public enum CellBehavior
  {
    Empty,
    Wall,
    Prefab,
    Door,
    PlayerStart,
    RewardPoint,
    EnemySpawner,
    NextLevel,
  }

  [Serializable]
  public class CellMapping
  {
    public string note;
    public int id;
    public CellBehavior behavior;
    public TileBase tile;
    public GameObject prefab;
  }

  public class RoomBuilder : MonoBehaviour
  {
    [Header("Base Tilemaps")]
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private TileBase floorTile;

    [SerializeField]
    private TileBase defaultWallTile;

    [Header("Procedural Perimeter (Edges)")]
    [SerializeField]
    private TileBase topLeftCorner;

    [SerializeField]
    private TileBase topRightCorner;

    [SerializeField]
    private TileBase bottomLeftCorner;

    [SerializeField]
    private TileBase bottomRightCorner;

    [Space]
    [Tooltip("Массивы для узоров 1-2-3. Будут повторяться через %")]
    [SerializeField]
    private TileBase[] topWallPatterns;

    [SerializeField]
    private TileBase[] bottomWallPatterns;

    [SerializeField]
    private TileBase[] leftWallPatterns;

    [SerializeField]
    private TileBase[] rightWallPatterns;

    [Header("Door Settings")]
    [SerializeField]
    private GameObject doorPrefab;

    [Header("Internal ID Configuration")]
    [SerializeField]
    private List<CellMapping> cellMappings = new List<CellMapping>();

    [Header("Camera")]
    [SerializeField]
    private PolygonCollider2D cameraBoundsCollider;

    private Dictionary<int, CellMapping> _mapConfig = new Dictionary<int, CellMapping>();
    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private List<DungeonDoor> _dungeonDoors = new List<DungeonDoor>();
    private DungeonRoomData _currentData;

    public Transform PlayerStartPoint { get; private set; }
    public Transform RewardPoint { get; private set; }
    public List<Transform> SpawnPoints { get; private set; } = new List<Transform>();

    private void Awake()
    {
      foreach (var mapping in cellMappings)
      {
        if (!_mapConfig.ContainsKey(mapping.id))
          _mapConfig.Add(mapping.id, mapping);
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

      int centerX = data.width / 2;
      int centerY = data.height / 2;

      for (int y = 0; y < data.height; y++)
      {
        for (int x = 0; x < data.width; x++)
        {
          Vector3Int tilePos = new Vector3Int(x, y, 0);
          floorTilemap.SetTile(tilePos, floorTile);

          // --- 1. ПРОВЕРКА ПЕРИМЕТРА (Края комнаты) ---
          bool isLeft = (x == 0);
          bool isRight = (x == data.width - 1);
          bool isBottom = (y == 0);
          bool isTop = (y == data.height - 1);

          if (isLeft || isRight || isBottom || isTop)
          {
            HandlePerimeter(x, y, tilePos, centerX, centerY, hasNorth, hasSouth, hasEast, hasWest);
            continue; // Игнорируем ID из TXT для краев
          }

          // --- 2. ВНУТРЕННЯЯ ЧАСТЬ (ID из конфига) ---
          int id = data.GetID(x, y);
          if (!_mapConfig.TryGetValue(id, out CellMapping mapping))
            continue;

          ProcessInternalCell(mapping, tilePos);
        }
      }

      ApplyDoorState(isCleared);
      UpdateCameraBounds();
    }

    private void HandlePerimeter(
      int x,
      int y,
      Vector3Int tilePos,
      int centerX,
      int centerY,
      bool hasN,
      bool hasS,
      bool hasE,
      bool hasW
    )
    {
      // УГЛЫ
      if (x == 0 && y == _currentData.height - 1)
      {
        wallTilemap.SetTile(tilePos, topLeftCorner);
        return;
      }
      if (x == _currentData.width - 1 && y == _currentData.height - 1)
      {
        wallTilemap.SetTile(tilePos, topRightCorner);
        return;
      }
      if (x == 0 && y == 0)
      {
        wallTilemap.SetTile(tilePos, bottomLeftCorner);
        return;
      }
      if (x == _currentData.width - 1 && y == 0)
      {
        wallTilemap.SetTile(tilePos, bottomRightCorner);
        return;
      }

      // ДВЕРИ ИЛИ СТЕНЫ (в центрах сторон)
      if (y == _currentData.height - 1 && x == centerX)
      {
        PlaceDoorOrWall(tilePos, hasN, DoorDirection.North, topWallPatterns, x);
        return;
      }
      if (y == 0 && x == centerX)
      {
        PlaceDoorOrWall(tilePos, hasS, DoorDirection.South, bottomWallPatterns, x);
        return;
      }
      if (x == 0 && y == centerY)
      {
        PlaceDoorOrWall(tilePos, hasW, DoorDirection.West, leftWallPatterns, y);
        return;
      }
      if (x == _currentData.width - 1 && y == centerY)
      {
        PlaceDoorOrWall(tilePos, hasE, DoorDirection.East, rightWallPatterns, y);
        return;
      }

      // ОБЫЧНЫЕ СТЕНЫ (с узором)
      TileBase wallTile = defaultWallTile;
      if (y == _currentData.height - 1)
        wallTile = GetPatternTile(topWallPatterns, x);
      else if (y == 0)
        wallTile = GetPatternTile(bottomWallPatterns, x);
      else if (x == 0)
        wallTile = GetPatternTile(leftWallPatterns, y);
      else if (x == _currentData.width - 1)
        wallTile = GetPatternTile(rightWallPatterns, y);

      wallTilemap.SetTile(tilePos, wallTile);
    }

    private void PlaceDoorOrWall(
      Vector3Int tilePos,
      bool hasDoor,
      DoorDirection dir,
      TileBase[] patterns,
      int index
    )
    {
      if (hasDoor)
      {
        SpawnDoor(doorPrefab, tilePos, dir);
      }
      else
      {
        wallTilemap.SetTile(tilePos, GetPatternTile(patterns, index));
      }
    }

    private TileBase GetPatternTile(TileBase[] patterns, int index)
    {
      if (patterns == null || patterns.Length == 0)
        return defaultWallTile;
      return patterns[index % patterns.Length];
    }

    private void ProcessInternalCell(CellMapping mapping, Vector3Int tilePos)
    {
      switch (mapping.behavior)
      {
        case CellBehavior.Wall:
          wallTilemap.SetTile(tilePos, mapping.tile);
          break;
        case CellBehavior.Prefab:
          Spawn(mapping.prefab, tilePos);
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

    public void ApplyDoorState(bool isCleared)
    {
      foreach (var door in _dungeonDoors)
      {
        if (isCleared)
          door.SetOpen();
        else
          door.SetClosed();
      }
    }

    // --- Вспомогательные методы (базовые) ---

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
      _spawnedObjects.Add(Instantiate(prefab, worldPos, Quaternion.identity, transform));
    }

    private Transform CreatePoint(string name, Vector3Int tilePos)
    {
      GameObject obj = new GameObject(name);
      obj.transform.position = floorTilemap.GetCellCenterWorld(tilePos);
      obj.transform.SetParent(transform);
      _spawnedObjects.Add(obj);
      return obj.transform;
    }

    private void CreateSpawnPoint(Vector3Int tilePos)
    {
      Transform point = CreatePoint($"SpawnPoint_{SpawnPoints.Count}", tilePos);
      SpawnPoints.Add(point);
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
        if (obj != null)
          Destroy(obj);
      _spawnedObjects.Clear();
    }

    public void UpdateCameraBounds()
    {
      if (cameraBoundsCollider == null)
        return;
      floorTilemap.CompressBounds();
      Bounds bounds = floorTilemap.localBounds;
      cameraBoundsCollider.points = new Vector2[]
      {
        new Vector2(bounds.min.x, bounds.min.y),
        new Vector2(bounds.min.x, bounds.max.y),
        new Vector2(bounds.max.x, bounds.max.y),
        new Vector2(bounds.max.x, bounds.min.y),
      };
    }
  }
}
