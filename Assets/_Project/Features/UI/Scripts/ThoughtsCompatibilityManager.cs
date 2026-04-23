using System;
using System.Collections.Generic;
using CherryFramework.DependencyManager;
using UnityEngine;

public class ThoughtsCompatibilityManager : InjectMonoBehaviour
{
  private HashSet<ArtifactInstance> _activeArtifacts = new();
  public IReadOnlyCollection<ArtifactInstance> ActiveArtifacts => _activeArtifacts;

  [Inject]
  private PlayerAccessor _playerAccessor;

  private ArtifactInstance _heart;
  private ArtifactInstance _weapon;
  private ArtifactInstance _accessory;

  public event Action OnActiveArtifactsChanged;
  public event Action OnThoughtDeselected;

  public bool HasPinnedArtifact { get; set; }

  public void AddPinnedArtifact(ArtifactInstance artifact)
  {
    _activeArtifacts.Add(artifact);
    HasPinnedArtifact = true;
    OnActiveArtifactsChanged?.Invoke();
  }

  public void RemovePinnedArtifact(ArtifactInstance artifact) {
    _activeArtifacts.Remove(artifact);
    HasPinnedArtifact = false;
    OnActiveArtifactsChanged?.Invoke();
  }

  public bool ContainsArtifact(ArtifactInstance artifact) {
    return _activeArtifacts.Contains(artifact);
  }

  public void ClearActiveArtifacts() {
    _activeArtifacts.Clear();
    OnActiveArtifactsChanged?.Invoke();
  }

  public bool IfNoActiveArtifacts() {
    return _activeArtifacts.Count == 0;
  }

  public void ActivateForThought(ThoughtType thoughtType) {
    if (HasPinnedArtifact) return;

    _heart = _playerAccessor.Equipment.Heart;
    _weapon = _playerAccessor.Equipment.Weapon;
    _accessory = _playerAccessor.Equipment.Accessory;

    if (thoughtType == ThoughtType.Absolute || thoughtType == ThoughtType.Fluid) {
      ClearActiveArtifacts();
      return;
    }

    if (thoughtType == ThoughtType.Heart) {
      _activeArtifacts = new HashSet<ArtifactInstance> { _heart };
    }
    else if (thoughtType == ThoughtType.Weapon) {
      _activeArtifacts = new HashSet<ArtifactInstance> { _weapon };
    }
    else if (thoughtType == ThoughtType.Accessory) {
      _activeArtifacts = new HashSet<ArtifactInstance> { _accessory };
    }

    OnActiveArtifactsChanged?.Invoke();
  }

  public void DeactivateForThoughts() {
    if (HasPinnedArtifact) return;

    ClearActiveArtifacts();
    OnThoughtDeselected?.Invoke();
  }
}
