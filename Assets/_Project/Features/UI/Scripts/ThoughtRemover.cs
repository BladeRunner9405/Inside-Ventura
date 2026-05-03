using CherryFramework.DependencyManager;
using InsideVentura.World;
using UnityEngine;

public class ThoughtRemover : InjectMonoBehaviour {
  [SerializeField]
  private GameObject thoughtItemPrefab;
  [SerializeField]
  private float dropSpawnDistance = 1.5f;

  [Inject]
  private PlayerAccessor _playerAccessor;

  public bool IsPointerInsideArea { get; set; } = false;

  public void DropThoughtToWorld(Thought thought) {
    Transform playerTransform = _playerAccessor.Transform;

    Vector3 spawnPosition = playerTransform.position + (Vector3)Random.insideUnitCircle.normalized * dropSpawnDistance;
    GameObject newThought = Instantiate(thoughtItemPrefab, spawnPosition, transform.rotation);

    ThoughtItem thoughtItem = newThought.GetComponent<ThoughtItem>();
    if (thoughtItem != null)
    {
      thoughtItem.Setup(thought);
    }
  }
}
