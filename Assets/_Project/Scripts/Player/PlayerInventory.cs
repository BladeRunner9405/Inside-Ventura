using UnityEngine;

public class PlayerInventory : MonoBehaviour {
  [SerializeField] private ThoughtBag thoughtBag;

  public ThoughtBag ThoughtBag => thoughtBag;

  private void Start() {
    if (thoughtBag) {
      thoughtBag = Instantiate(thoughtBag);
      thoughtBag.Initialize();
    }
  }

  public void AddThoughtToBag(Thought thought, int slotIndex = -1) {
    if (!thought) return;
    thoughtBag.TryAddThought(thought);
  }

  public void RemoveThoughtFromBag(Thought thought) {
    if (!thought) return;
    thoughtBag.RemoveThought(thought);
  }

  public void RemoveThoughtFromBag(int slotIndex) {
    thoughtBag.RemoveThoughtAt(slotIndex);
  }
}
