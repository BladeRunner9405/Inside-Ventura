using CherryFramework.DependencyManager;
using UnityEngine;

namespace InsideVentura.World
{
  public abstract class Item : InteractableObject {
    [Inject] private World.DungeonManager _dungeonManager;

    private void Start() {
      _dungeonManager.RegisterRoomObject(gameObject);
    }

    protected virtual void OnPickup()
    {
      AudioManager.Instance.PlaySFXpickup();
    }

    protected virtual bool CanPickUp()
    {
      // хватает ли места и т. п.
      return true;
    }

    public override void OnInteract()
    {
      if (!CanPickUp())
        return;

      OnPickup();
      Deactivate();
    }
  }
}
