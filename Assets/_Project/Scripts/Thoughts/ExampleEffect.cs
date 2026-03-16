using UnityEngine;

[CreateAssetMenu(fileName = "ExampleEffect", menuName = "Inside-Ventura/Effects/ExampleEffect")]
public class ExampleEffect : Effect {
  [SerializeField] private float damageBonus = 5f;
  [SerializeField] private StatModifierType modifierType = StatModifierType.Add;

  private StatModifier modifier;

  public override void OnEquipThought(Artifact artifact) {
    Debug.Log($"ExampleEffect экипирован на {artifact.name}");

    if (artifact is Weapon weapon) {
      modifier = new StatModifier(damageBonus, modifierType, this);
      weapon.AddDamageModifier(modifier);
      Debug.Log($"Урон {artifact.name} теперь {weapon.Damage}");
    }
  }

  public override void OnUnequipThought(Artifact artifact) {
    Debug.Log($"ExampleEffect снят с {artifact.name}");

    if (artifact is Weapon weapon && modifier != null) {
      weapon.RemoveDamageModifier(modifier);
      Debug.Log($"Урон {artifact.name} теперь {weapon.Damage}");
    }
  }
}
