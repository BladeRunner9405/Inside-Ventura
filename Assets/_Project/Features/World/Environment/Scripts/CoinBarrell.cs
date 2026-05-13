using CherryFramework.DependencyManager;
using InsideVentura.World;
using UnityEngine;

public class CoinBarrell : Entity {
  // [SerializeField] private int quantity;
  [SerializeField] private float maxSpawnDistance;
  [SerializeField] private MoneyItem moneyPrefab;

  [Header("Chances data")]
  [SerializeField] private LevelChancesData levelChances;

  [Header("Animation")]
  [SerializeField] private Sprite openedSprite;

  [Inject] protected PlayerAccessor PlayerAccessor;

  protected override void Start()
  {
    base.Start();
    if (PlayerAccessor != null)
      TargetTo(PlayerAccessor.Transform);
  }

  private new void Awake() {
    base.Awake();

    OnDeath += () =>
    {
      var quantity = levelChances.GetCurrentChances().Money.Roll();
      for (var i = 0; i < quantity; i++) {
        var spawnPosition = transform.position
                            + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);

        Instantiate(moneyPrefab, spawnPosition, transform.rotation);
        // Destroy(gameObject);
      }

      Open();
    };
  }

  private void Open()
  {
    GetComponentInChildren<SpriteRenderer>().sprite = openedSprite;
    GetComponent<Rigidbody2D>().simulated = false;
  }
}
