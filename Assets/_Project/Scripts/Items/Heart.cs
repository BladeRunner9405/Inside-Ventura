using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "Inside-Ventura/Artifacts/Heart")]
public class Heart : Artifact {
  [SerializeReference] private Effect[] traits;

  public IReadOnlyList<Effect> Traits => traits;

  public override void Initialize() {
    base.Initialize();
    ApplyHeartEffects();
  }

  public void ApplyHeartEffects() {
    if (Traits != null)
      foreach (var effect in Traits)
        effect.OnEquipThought(this);
  }
}
