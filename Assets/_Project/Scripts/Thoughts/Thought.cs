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
  [SerializeField] private string thoughtName;

  [SerializeReference] private Effect[] effects;
  [SerializeField] private Sprite inventoryIcon;
  [SerializeField] private int rarityLevel = 1;
  // [SerializeField] private string thoughtContent;

  [SerializeField] private ThoughtType type;

  public IReadOnlyList<Effect> Effects => effects;

  public bool HasRightType(Artifact artifact) {
    if (type == ThoughtType.Weapon && artifact is not Weapon) return false;
    if (type == ThoughtType.Heart && artifact is not Heart) return false;
    if (type == ThoughtType.Accessory && artifact is not Accessory) return false;
    return true;
  }

  public void OnEquip(Artifact artifact) {
    if (Effects != null)
      foreach (var effect in Effects)
        effect.OnEquipThought(artifact);
  }

  public void OnUnequip(Artifact artifact) {
    if (Effects != null)
      foreach (var effect in Effects)
        effect.OnUnequipThought(artifact);
  }
}
