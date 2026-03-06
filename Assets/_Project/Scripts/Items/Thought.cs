using System.Collections.Generic;
using UnityEngine;

public enum ThoughtType {
  Weapon, // Точный тип — только для оружия
  Heart, // Точный тип — только для сердца
  Accessory, // Точный тип — только для аксессуара
  Fluid, // Флюидный тип — эффект меняется в зависимости от артефакта
  Absolute // Абсолютный тип — любой артефакт, эффект всегда один и тот же
}

[CreateAssetMenu(fileName = "NewThought", menuName = "Inside-Ventura/Thought")]
public class Thought : ScriptableObject {
  [SerializeField] private string description;

  [SerializeReference] private Effect[] effects;
  [SerializeField] private Sprite inventoryIcon;
  [SerializeField] private int rarityLevel = 1;
  [SerializeField] private string thoughtContent;

  [SerializeField] private ThoughtType type;

  public IReadOnlyList<Effect> Effects => effects;

  public void Equip(Artifact artifact, GameObject player) {
    if (Effects != null)
      foreach (var effect in Effects)
        effect.OnEquipThought(artifact, player);
  }

  public void Unequip(Artifact artifact, GameObject player) {
    if (Effects != null)
      foreach (var effect in Effects)
        effect.OnUnequipThought(artifact, player);
  }
}
