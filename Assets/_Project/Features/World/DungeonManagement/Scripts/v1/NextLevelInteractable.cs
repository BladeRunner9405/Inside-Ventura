using CherryFramework.DependencyManager;
using UnityEngine;

namespace InsideVentura.World.v1
{
  public class NextLevelInteractable : InteractableObject
  {
    [Inject]
    private World.DungeonManager _dungeonManager;

    public override void OnInteract()
    {
      // Блокируем повторное нажатие
      if (!IsActive())
        return;

      if (_dungeonManager != null)
      {
        Debug.Log("<color=magenta>[Dungeon]</color> Спускаемся на следующий этаж...");
        _dungeonManager.RestartLevel();
      }

      base.OnInteract(); // Вызовет твой SetActive(false) и Destroy
    }
  }
}
