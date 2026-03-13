using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewThoughtBag", menuName = "Inside-Ventura/ThoughtBag")]
public class ThoughtBag : ScriptableObject {
  [SerializeField] private List<Thought> thoughts = new();
  [SerializeField] private int maxSize = 99;

  public IReadOnlyList<Thought> Thoughts => thoughts;

  public event Action OnThoughtsChanged;

  public void Initialize() {
    Clear();
  }

  public bool TryAddThought(Thought thought) {
    if (!thought) return false;
    if (thoughts.Count >= maxSize) {
      Debug.Log("Не удалось добавить мысль: переполнение.");
      return false;
    }

    thoughts.Add(thought);
    OnThoughtsChanged?.Invoke();
    return true;
  }

  public bool RemoveThought(Thought thought) {
    var removed = thoughts.Remove(thought);
    if (removed)
      OnThoughtsChanged?.Invoke();
    return removed;
  }

  public bool RemoveThoughtAt(int index) {
    if (index < 0 || index >= thoughts.Count) return false;
    thoughts.RemoveAt(index);
    OnThoughtsChanged?.Invoke();
    return true;
  }

  public void Clear() {
    thoughts.Clear();
    OnThoughtsChanged?.Invoke();
  }
}
