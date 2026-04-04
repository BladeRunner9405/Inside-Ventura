using Unity.VisualScripting;

namespace InsideVentura.World
{
  public abstract class Item : InteractableObject
{
  protected virtual void OnPickup() { }

  protected virtual bool CanPickUp()
  {
    // хватает ли места и т. п.
    return true;
  }

  protected override void OnEnable() {
    base.OnEnable();
    DungeonManager.Instance.RegisterRoomObject(this.gameObject);
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