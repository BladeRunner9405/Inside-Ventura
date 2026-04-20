using System;
using System.Collections.Generic;
using CherryFramework.DependencyManager;
using UnityEngine;

public class ThoughtsCompatibilityManager : InjectMonoBehaviour
{
  private HashSet<ArtifactInstance> _activeArtifacts = new();
  public IReadOnlyCollection<ArtifactInstance> ActiveArtifacts => _activeArtifacts;

  public event Action<ArtifactInstance> OnActiveArtifactsChanged;

  public void AddActiveArtifact(ArtifactInstance artifact)
  {
    _activeArtifacts.Add(artifact);
    OnActiveArtifactsChanged?.Invoke(artifact);
  }

  public void RemoveActiveArtifact(ArtifactInstance artifact) {
    _activeArtifacts.Remove(artifact);
    OnActiveArtifactsChanged?.Invoke(artifact);
  }

  public bool ContainsArtifact(ArtifactInstance artifact) {
    return _activeArtifacts.Contains(artifact);
  }

  public bool IfNoActiveArtifacts() {
    return _activeArtifacts.Count == 0;
  }

  /*public void ActivateForThought(ThoughtType thoughtType) {
    if (thoughtType == ThoughtType.Absolute || thoughtType == ThoughtType.Fluid) {
      return;
    }

    //...
  }*/
}
