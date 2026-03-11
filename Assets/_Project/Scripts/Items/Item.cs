public abstract class Item : InteractableObject {
  protected virtual void OnPickup() {
  }

  protected virtual bool CanPickUp() {
    // хватает ли места и т. п.
    return true;
  }

  public override void OnInteract() {
    if (!CanPickUp()) return;

    OnPickup();
    base.OnInteract();
  }
}
