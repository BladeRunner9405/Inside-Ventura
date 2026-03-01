using UnityEngine;

public class Thought : MonoBehaviour {
  [SerializeField] private readonly ThoughtData _data;

  public Thought(ThoughtData data) {
    _data = data;
  }

  public void Equip(Artifact artifact, GameObject player) {
    if (_data?.Effects != null) {
      foreach (var effect in _data.Effects)
        effect.OnEquipThought(artifact, player);
    }
  }

  public void Unequip(Artifact artifact, GameObject player) {
    if (_data?.Effects != null) {
      foreach (var effect in _data.Effects)
        effect.OnUnequipThought(artifact, player);
    }
  }
}
