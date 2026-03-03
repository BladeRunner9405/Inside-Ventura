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

  private void Start() {
    // это капец важно для того, чтобы не менялся оригинальный ScriptableObject
    if (weapon != null) {
      weapon = Instantiate(weapon);
      weapon.Initialize();
    }
  }

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

  public void Attack(Vector2 direction) {
    weapon?.Attack(gameObject, direction);
  }

  public void UseAbility() {
    accessory?.UseAbility();
  }
}
