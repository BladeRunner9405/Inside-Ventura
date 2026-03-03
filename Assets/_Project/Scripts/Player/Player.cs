using UnityEngine;

public class Player : Entity
{
    [SerializeField] private ItemPickup itemPickup;
    public void TryToInteract() {
        itemPickup.TryToInteract();
    }
}
