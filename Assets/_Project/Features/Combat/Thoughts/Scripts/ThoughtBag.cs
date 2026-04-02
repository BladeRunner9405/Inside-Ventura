using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewThoughtBag", menuName = "Inside-Ventura/ThoughtBag")]
public class ThoughtBag : ScriptableObject
{
  [SerializeField]
  private List<Thought> thoughts = new();

  [SerializeField]
  private int maxSize = 20;

  public IReadOnlyList<Thought> Thoughts => thoughts;
  public int MaxSize => maxSize;

  public event Action OnThoughtsChanged;

  public void Initialize() => Clear();

  public bool CanAddThought() => thoughts.Count < maxSize;

  public void AddThought(Thought thought)
  {
    if (!thought)
      return;

    if (thoughts.Contains(thought))
    {
      Debug.LogWarning(
        $"[ThoughtBag] Мысль «{thought.name}» уже в инвентаре — добавление отменено."
      );
      return;
    }

    if (!CanAddThought())
    {
      Debug.LogWarning("[ThoughtBag] Инвентарь переполнен.");
      return;
    }

    thoughts.Add(thought);
    OnThoughtsChanged?.Invoke();
  }

  public bool RemoveThought(Thought thought)
  {
    var removed = thoughts.Remove(thought);
    if (removed)
      OnThoughtsChanged?.Invoke();
    return removed;
  }

  public bool RemoveThoughtAt(int index)
  {
    if (index < 0 || index >= thoughts.Count)
      return false;
    thoughts.RemoveAt(index);
    OnThoughtsChanged?.Invoke();
    return true;
  }

  public void Clear()
  {
    thoughts.Clear();
    OnThoughtsChanged?.Invoke();
  }
}
