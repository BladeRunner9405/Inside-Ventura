using System.Collections.Generic;
using UnityEngine;

public enum ThoughtType {
  Weapon, // Точный тип — только для оружия
  Heart, // Точный тип — только для сердца
  Accessory, // Точный тип — только для аксессуара
  Fluid, // Флюидный тип — эффект меняется в зависимости от артефакта
  Absolute, // Абсолютный тип — любой артефакт, эффект всегда один и тот же
}

[CreateAssetMenu(fileName = "NewThought", menuName = "Inside-Ventura/ThoughtData")]
public class ThoughtData : ScriptableObject {
  [SerializeField] private string thoughtContent;
  [SerializeField] private string description;
  [SerializeField] private Sprite inventoryIcon;

  [SerializeField] private ThoughtType type;
  [SerializeField] private int rarityLevel = 1;

  [SerializeReference] private Effect[] effects;

  public IReadOnlyList<Effect> Effects => effects;
}
