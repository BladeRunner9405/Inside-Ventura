using UnityEngine;

public class Thought : MonoBehaviour
{
  [SerializeField] private readonly ThoughtData _data;

  public Thought(ThoughtData data)
  {
    _data = data;
  }

  public void Equip(Artifact artifact, IPlayer player) {
    if (_data?.effects != null)
    {
      foreach (var effect in _data.effects)
        effect.OnEquipThought(this, artifact, player);
    }
  }

  public void Unequip(Artifact artifact, IPlayer player) {
    if (_data?.effects != null) {
      foreach (var effect in _data.effects)
        effect.OnUnequipThought(this, artifact, player);
    }
  }
}
