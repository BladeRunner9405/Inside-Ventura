using CherryFramework.DependencyManager;
using InsideVentura.World;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : InteractableObject
{
  [SerializeField] private float maxSpawnDistance;

  [Header("Prefabs")]
  [SerializeField] private GameObject moneyItemPrefab;
  //[SerializeField] private GameObject manaItemPrefab;
  [SerializeField] private GameObject healthItemPrefab;
  [SerializeField] private GameObject thoughtItemPrefab;

  [Header("Chances data")]
  [SerializeField] private LevelChancesData levelChances;
  [SerializeField] private AudioClip openSound;

  [Inject]
  private ThoughtsAvaliabilityManager thoughtsAvaliabilityManager;

  public override void OnInteract()
  {
    var chances = levelChances.GetCurrentChances();
    SpawnItems(chances);
    AudioManager.Instance.PlaySFX(openSound);
    base.OnInteract();
  }

  private void SpawnItems(RoomChancesData data)
  {
    SpawnStatItem(data.Money, moneyItemPrefab);
    // SpawnStat(data.Mana, manaItemPrefab);
    SpawnStatItem(data.Health, healthItemPrefab);
    SpawnThoughts(data.Thoughts);
  }

  private void SpawnStatItem(ItemChancesData chances, GameObject defaultPrefab)
  {
    if (defaultPrefab == null) return;

    int count = chances.Roll();
    for (int i = 0; i < count; ++i)
    {
      var prefab = chances.PickPrefab(defaultPrefab);
      Instantiate(prefab, GetNewSpawnPosition(), transform.rotation);
    }
  }

  private void SpawnThoughts(ItemChancesData chances)
  {
    if (thoughtItemPrefab == null)
      return;

    int count = chances.Roll();
    for (int i = 0; i < count; i++)
    {
      Thought randomThought = thoughtsAvaliabilityManager.GetRandomThought();
      if (randomThought == null) continue;

      var newThought    = Instantiate(thoughtItemPrefab, GetNewSpawnPosition(), transform.rotation);

      newThought.GetComponent<ThoughtItem>().Setup(randomThought);
    }
  }

  private Vector3 GetNewSpawnPosition() =>
    transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);
}

