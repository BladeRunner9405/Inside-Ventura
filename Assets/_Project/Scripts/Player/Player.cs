using UnityEngine;

public class Player : Entity
{
    [SerializeField] private ItemPickup itemPickup;

    public void TryToInteract() {
        itemPickup.TryToInteract();
    }

    /*void Heal(int amount) {
        if (IsDead) return;
        if (amount <= 0) return;

        Health += amount;
    }

    void ModifyMaxHealth(int delta) {
      if (IsDead) return;

      MaxHealth += delta;
    }*/
}
