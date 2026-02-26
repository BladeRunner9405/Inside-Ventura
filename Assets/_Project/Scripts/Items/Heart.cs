using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "Inside-Ventura/Artifacts/Heart")]
public class Heart : Artifact
{
  [SerializeField] private readonly Dictionary<StatType, List<StatModifier>> _traits = new Dictionary<StatType, List<StatModifier>>();

  public void ApplyTraits(IPlayer player)
  {
    foreach (var kv in _traits)
    {
      foreach (var modifier in kv.Value)
        player.Stats.AddModifier(kv.Key, modifier);
    }
  }

  public void RemoveTraits(IPlayer player)
  {
    foreach (var kv in _traits)
    {
      foreach (var modifier in kv.Value)
        player.Stats.RemoveModifier(kv.Key, modifier);
    }
  }
}
