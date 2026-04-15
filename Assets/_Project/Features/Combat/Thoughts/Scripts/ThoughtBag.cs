using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewThoughtBag", menuName = "InsideVentura/ThoughtBag")]
public class ThoughtBag : ScriptableObject
{
  [SerializeField]
  private string inventoryName;
  [SerializeField]
  private string inventoryDescription;

  [SerializeField]
  private Thought[] thoughts;

  [SerializeField]
  private int maxSize = 20;

  public string Name => inventoryName;
  public string Description => inventoryDescription;
  public IReadOnlyList<Thought> Thoughts => Array.AsReadOnly(thoughts);
  public int MaxSize => maxSize;

  public event Action OnThoughtsChanged;

  public bool CanAddThought() => Array.IndexOf(thoughts, null) != -1;

  private void OnEnable()
  {
    if (thoughts == null || thoughts.Length != maxSize)
    {
      thoughts = new Thought[maxSize];
    }
  }

  public void Initialize() {
    // Clear();
  }

  public void SetThoughtAt(int index, Thought thought)
  {
    if (index < 0 || index >= maxSize) return;
    thoughts[index] = thought;
    OnThoughtsChanged?.Invoke();
  }

  public bool AddThought(Thought thought)
  {
    if (thought == null) return false;

    int emptyIndex = Array.IndexOf(thoughts, null);
    if (emptyIndex == -1)
    {
      Debug.LogWarning("[ThoughtBag] Инвентарь переполнен.");
      return false;
    }

    thoughts[emptyIndex] = thought;
    OnThoughtsChanged?.Invoke();

    return true;
  }

  public bool RemoveThought(Thought thought)
  {
    int index = Array.IndexOf(thoughts, thought);
    if (index == -1) return false;

    thoughts[index] = null;
    OnThoughtsChanged?.Invoke();

    return true;
  }

  public bool RemoveThoughtAt(int index)
  {
    if (index < 0 || index >= maxSize) return false;
    if (thoughts[index] == null) return false;

    thoughts[index] = null;
    OnThoughtsChanged?.Invoke();

    return true;
  }

  public void Clear()
  {
    Array.Clear(thoughts, 0, thoughts.Length);
    OnThoughtsChanged?.Invoke();
  }
}
