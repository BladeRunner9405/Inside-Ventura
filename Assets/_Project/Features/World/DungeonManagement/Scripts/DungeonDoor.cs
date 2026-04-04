using CherryFramework.DependencyManager;
using UnityEngine;

namespace InsideVentura.World
{
  public enum DoorDirection
  {
    North, // Индекс 0
    South, // Индекс 1
    East, // Индекс 2
    West, // Индекс 3
  }

  public class DungeonDoor : InteractableObject
  {
    public DoorDirection direction;

    [Header("Sprites (0: North, 1: South, 2: East, 3: West)")]
    [SerializeField]
    private Sprite[] closedDoors = new Sprite[4];

    [SerializeField]
    private Sprite[] openedDoors = new Sprite[4];

    [Space]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [Inject]
    private DungeonManager _dungeonManager;

    public void SetClosed()
    {
      if (closedDoors.Length > (int)direction)
      {
        Debug.Log(direction);
        spriteRenderer.sprite = closedDoors[(int)direction];
      }
    }

    public void SetOpen()
    {
      if (openedDoors.Length > (int)direction)
      {
        spriteRenderer.sprite = openedDoors[(int)direction];
      }
    }

    public override void OnInteract()
    {
      // Спрашиваем у менеджера, можно ли сейчас выходить
      if (_dungeonManager != null && _dungeonManager.CanExitRoom())
      {
        _dungeonManager.MoveToRoom(direction);
      }
      else
      {
        Debug.Log("<color=red>Дверь заперта!</color> Нужно победить всех врагов.");
        // Здесь можно запустить звук "дверь закрыта" или анимацию тряски
      }
    }
  }
}
