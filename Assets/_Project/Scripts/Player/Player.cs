using UnityEngine;

public class Player : Entity
{
    public ItemPickup itemPickup;
    public void TryToInteract() {
        itemPickup.TryToInteract();
    }
}
