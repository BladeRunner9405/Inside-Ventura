using System;
using CherryFramework.DependencyManager;

public class ThoughtsCompatibilityManager : InjectMonoBehaviour
{
  private ArtifactInstance _activeArtifact;
  public ArtifactInstance ActiveArtifact => _activeArtifact;

  public event Action<ArtifactInstance> OnActiveArtifactChanged;

  public void SetActiveArtifact(ArtifactInstance artifact)
  {
    if (_activeArtifact == artifact) return;
    _activeArtifact = artifact;
    OnActiveArtifactChanged?.Invoke(_activeArtifact);
  }

  public void ClearActiveArtifact() => SetActiveArtifact(null);
}
