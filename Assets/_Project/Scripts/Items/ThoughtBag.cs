using System.Collections.Generic;
using UnityEngine;

public class ThoughtBag : MonoBehaviour {
  [SerializeField] private List<Thought> thoughts = new List<Thought>();
  [SerializeField] private int maxSize = 99;

  public bool AddThought(Thought thought) {
    if (thoughts.Count >= maxSize) return false;

    thoughts.Add(thought);
    return true;
  }

  public bool RemoveThought(Thought thought) {
    return thoughts.Remove(thought);
  }
}
