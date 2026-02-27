using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
  Weapon Weapon { get; }
  Heart Heart { get; }
  Accessory Accessory { get; }

  void EquipThought(Artifact artifact, Thought thought, int slotIndex) {}

  void UnequipThought(Artifact artifact, int slotIndex) {}

  void Attack() {}

  void UseAbility() {}
}
