using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject {
  [SerializeField] private float maxSpawnDistance;
  public GameObject[] itemsToSpawn; // мы реализуем по-другому, это заглушка
  public int quantity;

  public override void OnInteract() {
    SpawnItems();
    base.OnInteract();
  }

  private void SpawnItems() {
    List<GameObject> available = new List<GameObject>(itemsToSpawn);
    int spawnCount = Mathf.Min(quantity, available.Count);

    for (int i = 0; i < spawnCount; ++i) {
      int randIndex = Random.Range(0, available.Count);
      GameObject item = available[randIndex];
      available.RemoveAt(randIndex);

      Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);
      Instantiate(item, pos, transform.rotation);
    }
  }
}
