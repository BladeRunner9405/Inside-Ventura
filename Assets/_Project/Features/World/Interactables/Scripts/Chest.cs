using UnityEngine;

public class Chest : InteractableObject
{
  [SerializeField]
  private float maxSpawnDistance;
  public GameObject[] itemsToSpawn; // мы реализуем по-другому, это заглушка
  public int quantity;

  public override void OnInteract()
  {
    SpawnItems();
    base.OnInteract();
  }

  private void SpawnItems()
  {
    for (int i = 0; i < quantity; ++i)
    {
      GameObject randomItem = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];
      Vector3 spawnPosition =
        transform.position
        + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);
      Instantiate(randomItem, spawnPosition, transform.rotation);
    }
  }
}
