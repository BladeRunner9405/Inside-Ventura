using UnityEngine;
using CherryFramework.DependencyManager; // Добавляем пространство имен

namespace InsideVentura.World
{
    public enum DoorDirection { North, South, East, West }

    // InteractableObject уже наследуется от InjectMonoBehaviour, так что [Inject] сработает
    public class DungeonDoor : InteractableObject
    {
        public DoorDirection direction;
        [Inject] private DungeonManager _dungeonManager;

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