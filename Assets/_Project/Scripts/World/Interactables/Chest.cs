using UnityEngine;

public class Chest : InteractableObject {
  [SerializeField] private float maxSpawnDistance;
  public GameObject itemToSpawn; // конечно же мы реализуем по-другому, это заглушка
  public int quantity;


  public override void OnInteract() {
    SpawnItems();
    base.OnInteract();
  }

  private void SpawnItems() {
    for (var i = 0; i < quantity; i++)
      Instantiate(itemToSpawn, transform.position +
                               (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance),
        transform.rotation);
  }
}
