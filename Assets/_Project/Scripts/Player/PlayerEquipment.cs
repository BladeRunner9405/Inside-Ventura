using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour {
  [SerializeField] private Weapon weapon;
  [SerializeField] private Heart heart;
  [SerializeField] private Accessory accessory;

  public Weapon Weapon => weapon;
  public Heart Heart => heart;
  public Accessory Accessory => accessory;

  public event Action<Artifact, int, Thought> OnThoughtEquipped;
  public event Action<Artifact, int> OnThoughtUnequipped;

  public void EquipThought(Artifact artifact, Thought thought, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= artifact.SlotsCount) return;

    artifact.EquipThought(thought, slotIndex, gameObject);

    OnThoughtEquipped?.Invoke(artifact, slotIndex, thought);
  }

  void UnequipThought(Artifact artifact, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= artifact.SlotsCount) return;

    artifact.UnequipThought(slotIndex, gameObject);

    OnThoughtUnequipped?.Invoke(artifact, slotIndex);
  }

  public void Attack() {
    weapon?.Attack();
  }

  public void UseAbility() {
    accessory?.UseAbility();
  }
}
