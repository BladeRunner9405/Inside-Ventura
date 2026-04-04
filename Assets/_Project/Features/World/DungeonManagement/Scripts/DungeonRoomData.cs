using UnityEngine;

namespace InsideVentura.World
{
  [CreateAssetMenu(fileName = "NewRoomData", menuName = "InsideVentura/Dungeon/RoomData")]
  public class DungeonRoomData : ScriptableObject
  {
    [Header("Room Dimensions")]
    public int width = 17; // В Isaac обычно нечетные размеры для центральных дверей
    public int height = 9;

    [Header("Layout (0-Empty, 1-Wall, 2-Anger, 3-Envy, 4-Obstacle)")]
    // Используем одномерный массив, так как Unity лучше его сериализует
    public int[] layout;

    public int GetID(int x, int y)
    {
      int index = y * width + x;
      if (index >= 0 && index < layout.Length)
        return layout[index];
      return 1; // Возвращаем стену, если вышли за границы
    }
  }
}
