using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "Inside-Ventura/Artifacts/Heart")]
public class Heart : Artifact {
  [SerializeReference] private Effect[] traits;

  public IReadOnlyList<Effect> Traits => traits;

  public override void Initialize() {
    base.Initialize();

    if (traits != null) {
      var cloned = new Effect[traits.Length];
      for (var i = 0; i < traits.Length; ++i) cloned[i] = Instantiate(traits[i]);
      traits = cloned;
    }

    ApplyHeartEffects();
  }

  public void ApplyHeartEffects() {
    if (Traits != null)
      foreach (var effect in Traits)
        effect.OnEquipThought(this);
  }
}
