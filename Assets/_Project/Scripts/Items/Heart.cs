using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "Inside-Ventura/Artifacts/Heart")]
public class Heart : Artifact
{
  [SerializeField] private readonly Dictionary<Stat, List<StatModifier>> _traits = new();

  public void ApplyHeartEffects(PlayerStats stats) {
  }
}
