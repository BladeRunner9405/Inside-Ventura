using CherryFramework.DependencyManager;
using UnityEngine;

namespace InsideVentura.World.v1
{
  public abstract class Item : InteractableObject {
    [Inject] private World.DungeonManager _dungeonManager;

    private void Start() {
      _dungeonManager.RegisterRoomObject(gameObject);
    }

    protected virtual void OnPickup() { }

    protected virtual bool CanPickUp()
    {
      // хватает ли места и т. п.
      return true;
    }

    protected override void OnEnable()
    {
      base.OnEnable();
    }

    public override void OnInteract()
    {
      if (!CanPickUp())
        return;

      OnPickup();
      base.OnInteract();
    }
  }
}
