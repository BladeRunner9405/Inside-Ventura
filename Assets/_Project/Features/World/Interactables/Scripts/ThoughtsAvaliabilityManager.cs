using System.Collections.Generic;
using UnityEngine;

public class ThoughtsAvaliabilityManager : MonoBehaviour
{
  [SerializeField] private ThoughtsLibrary thoughtsLibrary;

  private List<Thought> remainingThoughts;

  private void Awake()
  {
    ResetPool();
  }

  public void ResetPool()
  {
    remainingThoughts = new List<Thought>(thoughtsLibrary.allThoughts);
  }

  public Thought GetRandomThought()
  {
    if (remainingThoughts == null || remainingThoughts.Count == 0)
      return null;

    int index = Random.Range(0, remainingThoughts.Count);
    Thought thought = remainingThoughts[index];
    remainingThoughts.RemoveAt(index);
    return thought;
  }
}
