using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "Inside-Ventura/Artifacts/Heart")]
public class Heart : Artifact {
  [SerializeReference] private Effect[] traits;

  public IReadOnlyList<Effect> Traits => traits;

  public override void Initialize(GameObject player) {
    base.Initialize(player);
    ApplyHeartEffects(player);
  }

  public void ApplyHeartEffects(GameObject player) {
    if (Traits != null)
      foreach (var effect in Traits)
        effect.OnEquipThought(this, player);
  }
}
