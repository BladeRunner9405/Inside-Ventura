namespace InsideVentura.World
{
    public enum RoomType { Safe, Normal, Boss }

    public class RoomInstance
    {
        public DungeonRoomData Data { get; private set; }
        public RoomType Type { get; private set; }
        public bool IsCleared { get; set; }
        
        // ДОБАВЛЕНО: Сценарий боя для этой комнаты
        public EncounterData Encounter { get; private set; } 

        public RoomInstance(DungeonRoomData data, RoomType type, bool isCleared, EncounterData encounter = null)
        {
            Data = data;
            Type = type;
            IsCleared = isCleared;
            Encounter = encounter;
        }
    }
}