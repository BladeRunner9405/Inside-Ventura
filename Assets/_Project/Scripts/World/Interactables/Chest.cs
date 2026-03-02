using UnityEngine;

public class Chest: InteractableObject
{
    [SerializeField] private float maxSpawnDistance;
    public GameObject itemToSpawn; // конечно же мы реализуем по-другому, это заглушка
    public int quantity;


    public override void Interact(Player player) {
        SpawnItems();
        base.Interact(player);
    }

    private void SpawnItems() {
        for (int i=0; i<quantity; i++) {
            Instantiate(itemToSpawn, transform.position + 
                                        (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance), transform.rotation);
        }
    }
}
