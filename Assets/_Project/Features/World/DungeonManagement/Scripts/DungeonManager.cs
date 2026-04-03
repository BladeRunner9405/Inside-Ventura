using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine; // Для поиска врагов

namespace InsideVentura.World
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField] private RoomBuilder builder;
        [SerializeField] private int targetRoomCount = 8;
        [SerializeField] private DungeonRoomData safeRoomTemplate;
        [SerializeField] private List<DungeonRoomData> normalRoomTemplates;
        [SerializeField] private EncounterManager encounterManager;
        [SerializeField] private List<EncounterData> encounterPool;

        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab; // ПРЕФАБ ИГРОКА
        private GameObject _activePlayer;
        [Header("Camera")]
        [SerializeField] private CinemachineCamera _playerCamera;

        private Dictionary<Vector2Int, RoomInstance> _dungeonMap = new Dictionary<Vector2Int, RoomInstance>();
        private Vector2Int _currentCoords = Vector2Int.zero;

        // Список врагов в текущей комнате
        private List<Enemy> _enemiesInRoom = new List<Enemy>();

        private void Start()
        {
            GenerateDungeon();
            LoadRoom(_currentCoords);
            CinemachineConfiner2D confiner = _playerCamera.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                confiner.InvalidateBoundingShapeCache();
                Debug.Log("<color=cyan>[Camera]</color> Границы обновлены, кэш сброшен.");
            }
            InitialPlayerPlacement();
        }

        private void InitialPlayerPlacement()
        {
            if (playerPrefab != null && builder.PlayerStartPoint != null)
            {
                // Спавним игрока и запоминаем ссылку навсегда!
                _activePlayer = Instantiate(playerPrefab, builder.PlayerStartPoint.position, Quaternion.identity);
                _playerCamera.Follow = _activePlayer.GetComponent<Entity>().target;
                Debug.Log("<color=green>[Dungeon]</color> Игрок заспавнен как префаб.");
            }
            else
            {
                Debug.LogError("Не назначен префаб игрока или точка старта (ID 10)!");
            }
        }

        private void TeleportPlayer(DoorDirection enteredDoor)
        {
            if (_activePlayer == null) return; // Используем нашу ссылку!

            DoorDirection targetDoorDir = GetOppositeDirection(enteredDoor);
            DungeonDoor[] newDoors = FindObjectsOfType<DungeonDoor>(); 
            
            foreach (var door in newDoors)
            {
                if (door.direction == targetDoorDir)
                {
                    Vector3 offset = Vector3.zero;
                    float step = 1.5f;

                    switch (targetDoorDir)
                    {
                        case DoorDirection.North: offset = Vector3.down * step; break;
                        case DoorDirection.South: offset = Vector3.up * step; break;
                        case DoorDirection.East:  offset = Vector3.left * step; break;
                        case DoorDirection.West:  offset = Vector3.right * step; break;
                    }

                    // Перемещаем НАШЕГО игрока
                    _activePlayer.transform.position = door.transform.position + offset;
                    return;
                }
            }

            _activePlayer.transform.position = Vector3.zero;
        }

        // Метод, который вернет true, если врагов нет
        // Обнови проверку в CanExitRoom:
        public bool CanExitRoom()
        {
            // Если бой идет — двери заперты. Если нет — открыты.
            return !encounterManager.IsEncounterActive;
        }

        // Замени метод LoadRoom полностью:
        private void LoadRoom(Vector2Int coords)
        {
            RoomInstance instance = _dungeonMap[coords];

            bool hasNorth = _dungeonMap.ContainsKey(coords + Vector2Int.up);
            bool hasSouth = _dungeonMap.ContainsKey(coords + Vector2Int.down);
            bool hasEast  = _dungeonMap.ContainsKey(coords + Vector2Int.right);
            bool hasWest  = _dungeonMap.ContainsKey(coords + Vector2Int.left);

            builder.Build(instance.Data, instance.IsCleared, hasNorth, hasSouth, hasEast, hasWest);

            // Если комната не зачищена - стартуем бой через EncounterManager
            if (!instance.IsCleared && instance.Type != RoomType.Safe)
            {
                encounterManager.StartEncounter(
                    instance.Encounter, 
                    builder.SpawnPoints, // Передаем точки, которые собрал билдер (девятки из txt)
                    () => {
                        // Этот коллбэк вызовется, когда EncounterManager закончит все волны
                        _dungeonMap[_currentCoords].IsCleared = true;
                    }
                );
            }
        }

        private void CheckRoomClear(Enemy killedEnemy)
        {
            _enemiesInRoom.Remove(killedEnemy);

            if (_enemiesInRoom.Count == 0)
            {
                // Помечаем комнату в словаре как зачищенную НАВСЕГДА
                _dungeonMap[_currentCoords].IsCleared = true;
                Debug.Log("<color=cyan>[Battle]</color> Комната зачищена! Двери открыты.");
            }
        }

        public void MoveToRoom(DoorDirection doorDir)
        {
            // Старая логика перемещения координат...
            Vector2Int nextCoords = _currentCoords;
            switch (doorDir)
            {
                case DoorDirection.North: nextCoords += Vector2Int.up; break;
                case DoorDirection.South: nextCoords += Vector2Int.down; break;
                case DoorDirection.East:  nextCoords += Vector2Int.right; break;
                case DoorDirection.West:  nextCoords += Vector2Int.left; break;
            }

            if (_dungeonMap.ContainsKey(nextCoords))
            {
                _currentCoords = nextCoords;
                _enemiesInRoom.Clear(); 
                
                // УБИРАЕМ ТРУПЫ ИЗ ПРОШЛОЙ КОМНАТЫ В ПУЛ
                encounterManager.ClearCorpses(); 
                
                LoadRoom(_currentCoords);
                TeleportPlayer(doorDir);
            }
        }

        private void GenerateDungeon()
        {
            _dungeonMap.Clear();
            Vector2Int currentPos = Vector2Int.zero;

            // 1. Спавним стартовую (Безопасную) комнату
            // Она уже "зачищена" по умолчанию
            _dungeonMap.Add(currentPos, new RoomInstance(safeRoomTemplate, RoomType.Safe, true));

            int roomsCreated = 1;

            // 2. Алгоритм "Пьяная походка"
            while (roomsCreated < targetRoomCount)
            {
                // Выбираем случайное направление
                Vector2Int dir = GetRandomDirectionVector();
                Vector2Int newPos = currentPos + dir;

                // Если клетка пустая - создаем там комнату
                // В GenerateDungeon(), когда создаешь Normal комнаты, выдавай им случайный Encounter:
                if (!_dungeonMap.ContainsKey(newPos))
                {
                    var randomTemplate = normalRoomTemplates[Random.Range(0, normalRoomTemplates.Count)];
                    
                    // ДОБАВЛЕНО: берем случайную стычку из пула
                    EncounterData randomEncounter = encounterPool.Count > 0 ? 
                        encounterPool[Random.Range(0, encounterPool.Count)] : null;
                    
                    _dungeonMap.Add(newPos, new RoomInstance(randomTemplate, RoomType.Normal, false, randomEncounter));
                    roomsCreated++;
                }

                // Смещаем "строителя"
                currentPos = newPos;
            }

            Debug.Log($"<color=green>[DungeonGenerator]</color> Сгенерировано комнат: {roomsCreated}");
        }

        private Vector2Int GetRandomDirectionVector()
        {
            int r = Random.Range(0, 4);
            return r switch
            {
                0 => Vector2Int.up,
                1 => Vector2Int.down,
                2 => Vector2Int.left,
                3 => Vector2Int.right,
                _ => Vector2Int.up
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
                _ => DoorDirection.South
            };
        }

        public void GenerateNextLevel()
        {
            // 1. Убираем трупы и врагов из старой комнаты
            if (encounterManager != null)
            {
                encounterManager.ClearCorpses();
            }
            _enemiesInRoom.Clear();

            // 2. Сбрасываем карту и координаты
            _dungeonMap.Clear();
            _currentCoords = Vector2Int.zero;

            // 3. Генерируем новую карту и грузим стартовую комнату
            GenerateDungeon();
            LoadRoom(_currentCoords);

            // 4. Телепортируем игрока на точку старта (ID 2/10)
            if (_activePlayer != null && builder.PlayerStartPoint != null)
            {
                _activePlayer.transform.position = builder.PlayerStartPoint.position;
            }
        }
    }

    
}