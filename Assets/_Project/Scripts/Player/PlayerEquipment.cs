using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour {
  [SerializeField] private Weapon weapon;
  [SerializeField] private Heart heart;
  [SerializeField] private Accessory accessory;

  [Header("Debug")] [SerializeField] private Thought testThought;

  public Weapon Weapon => weapon;
  public Heart Heart => heart;
  public Accessory Accessory => accessory;

  private void Start() {
    // это капец важно для того, чтобы не менялся оригинальный ScriptableObject
    if (weapon) {
      weapon = Instantiate(weapon);
      weapon.Initialize(gameObject);
    }

    if (heart) {
      heart = Instantiate(heart);
      heart.Initialize(gameObject);
    }

    if (accessory) {
      accessory = Instantiate(accessory);
      accessory.Initialize(gameObject);
    }
  }

  public event Action<Artifact, int, Thought> OnThoughtEquipped;
  public event Action<Artifact, int> OnThoughtUnequipped;

  public void EquipThought(Artifact artifact, Thought thought, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= artifact.SlotsCount) return;

    artifact.EquipThought(thought, slotIndex, gameObject);

    OnThoughtEquipped?.Invoke(artifact, slotIndex, thought);
  }

  private void UnequipThought(Artifact artifact, int slotIndex) {
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


  [ContextMenu("Экипировать тестовую мысль в 0-ой слот оружия")]
  private void DebugEquipThoughtToWeaponSlot0() {
    if (!testThought || !weapon) return;
    EquipThought(weapon, testThought, 0);
  }

  [ContextMenu("Снять мысль с 0-ого слота оружия")]
  private void DebugUnequipThoughtToWeaponSlot0() {
    if (!weapon) return;
    UnequipThought(weapon, 0);
  }
}
