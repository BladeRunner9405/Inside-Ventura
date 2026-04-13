using System.Collections.Generic;
using UnityEngine;

namespace InsideVentura.World
{
  public enum RoomType
  {
    Safe,
    Normal,
    Boss,
  }

  public class RoomInstance
  {
    public DungeonRoomData Data;
    public RoomType Type;
    public bool IsCleared;
    public EncounterData Encounter;
    public bool Visited;

    // ДОБАВЛЕНО: Рюкзак для сундуков, монеток и предметов
    public List<GameObject> RoomObjects = new List<GameObject>();

    public RoomInstance(
      DungeonRoomData data,
      RoomType type,
      bool isCleared,
      EncounterData encounter = null
    )
    {
      Data = data;
      Type = type;
      IsCleared = isCleared;
      Encounter = encounter;
      Visited = false;
    }
  }
}
