using InsideVentura.World.v1;
using UnityEngine;

public class Chest : InteractableObject
{
  [SerializeField]
  private float maxSpawnDistance;
  public GameObject[] itemsToSpawn; // мы реализуем по-другому, это заглушка
  public int quantity;

  [SerializeField]
  private GameObject thoughtItemPrefab;
  [SerializeField]
  private Thought[] possibleThoughts;

  public override void OnInteract()
  {
    SpawnItems();
    base.OnInteract();
  }

  // тут скоро будет нормальная логика
  private void SpawnItems()
  {
    for (int i = 0; i < quantity; ++i)
    {
      Vector3 spawnPosition = transform.position
                              + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);

      SpawnRandomThought(spawnPosition);
      SpawnRandomItem(spawnPosition);
    }
  }
  private void SpawnRandomThought(Vector3 spawnPosition)
  {
    Thought randomThought = possibleThoughts[Random.Range(0, possibleThoughts.Length)];
    GameObject newThought = Instantiate(thoughtItemPrefab, spawnPosition, transform.rotation);

    ThoughtItem thoughtItem = newThought.GetComponent<ThoughtItem>();
    if (thoughtItem != null)
    {
      thoughtItem.Setup(randomThought);
    }
  }

  private void SpawnRandomItem(Vector3 spawnPosition)
  {
    GameObject randomItem = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];
    Instantiate(randomItem, spawnPosition, transform.rotation);
  }
}
