using System;
using CherryFramework.DependencyManager;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour {
  [SerializeField] private Weapon weapon;
  [SerializeField] private Heart heart;
  [SerializeField] private Accessory accessory;

  [Header("Debug")]
  [SerializeField] private Thought testThought;

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
    artifact.EquipThought(thought, slotIndex);

    OnThoughtEquipped?.Invoke(artifact, slotIndex, thought);
  }

  private void UnequipThought(Artifact artifact, int slotIndex) {
    artifact.UnequipThought(slotIndex);

    OnThoughtUnequipped?.Invoke(artifact, slotIndex);
  }

  public void TryToAttack(Vector2 direction) {
    weapon?.TryAttack(direction);
  }

  public void TryToUseAbility(Vector2 direction) {
    accessory?.TryUseAbility(direction);
  }



  // DebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebugDebug

  [ContextMenu("Экипировать тестовую мысль в 0-ой слот сердца")]
  public void DebugEquipThoughtToHeartSlot0() {
    if (heart.HasThought(testThought)) {
      UnequipThought(heart, 0);
      return;
    }
    EquipThought(heart, testThought, 0);
  }

  [ContextMenu("Экипировать тестовую мысль в 0-ой слот аксессуара")]
  public void DebugEquipThoughtToAccessorySlot0() {
    if (heart.HasThought(testThought)) {
      UnequipThought(accessory, 0);
      return;
    }
    EquipThought(accessory, testThought, 0);
  }

  [ContextMenu("Экипировать тестовую мысль в 0-ой слот оружия")]
  public void DebugEquipThoughtToWeaponSlot0() {
    if (heart.HasThought(testThought)) {
      UnequipThought(weapon, 0);
      return;
    }
    EquipThought(weapon, testThought, 0);
  }
}
