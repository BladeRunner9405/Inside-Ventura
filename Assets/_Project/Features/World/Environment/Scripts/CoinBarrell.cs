using InsideVentura.World.v1;
using UnityEngine;

public class CoinBarrell : Entity {
  [SerializeField] private int quantity;
  [SerializeField] private float maxSpawnDistance;
  [SerializeField] private MoneyItem moneyPrefab;

  private new void Awake() {
    base.Awake();

    OnDeath += () => {
      for (var i = 0; i < quantity; i++) {
        var spawnPosition = transform.position
                            + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);

        Instantiate(moneyPrefab, spawnPosition, transform.rotation);
        Destroy(gameObject);
      }
    };
  }
}
