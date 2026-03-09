using System;
using CherryFramework.DependencyManager;
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
      DependencyContainer.Instance.InjectDependencies(weapon);
      weapon.Initialize();
    }

    if (heart) {
      heart = Instantiate(heart);
      DependencyContainer.Instance.InjectDependencies(heart);
      heart.Initialize();
    }

    if (accessory) {
      accessory = Instantiate(accessory);
      DependencyContainer.Instance.InjectDependencies(accessory);
      accessory.Initialize();
    }
  }

  public event Action<Artifact, int, Thought> OnThoughtEquipped;
  public event Action<Artifact, int> OnThoughtUnequipped;

  public void EquipThought(Artifact artifact, Thought thought, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= artifact.SlotsCount) return;

    artifact.EquipThought(thought, slotIndex);

    OnThoughtEquipped?.Invoke(artifact, slotIndex, thought);
  }

  private void UnequipThought(Artifact artifact, int slotIndex) {
    if (slotIndex < 0 || slotIndex >= artifact.SlotsCount) return;

    artifact.UnequipThought(slotIndex);

    OnThoughtUnequipped?.Invoke(artifact, slotIndex);
  }

  public void TryToAttack(Vector2 direction) {
    weapon?.TryAttack(direction);
  }

  public void TryToUseAbility(Vector2 direction) {
    accessory?.TryUseAbility(direction);
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
