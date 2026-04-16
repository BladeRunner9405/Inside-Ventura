using System;
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
  [SerializeField] private Thought[]  possibleThoughts;

  [Header("Chances data")]
  [SerializeField] private LevelChancesData levelChances;

  public override void OnInteract()
  {
    var chances = levelChances.GetCurrentChances();
    SpawnItems(chances);

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
    if (thoughtItemPrefab == null || possibleThoughts == null || possibleThoughts.Length == 0)
      return;

    int count = chances.Roll();
    for (int i = 0; i < count; i++)
    {
      // TODO: вычеркивать выбранную мысль из списка, чтобы не повторялась
      var randomThought = possibleThoughts[Random.Range(0, possibleThoughts.Length)];
      var newThought    = Instantiate(thoughtItemPrefab, GetNewSpawnPosition(), transform.rotation);

      newThought.GetComponent<ThoughtItem>().Setup(randomThought);
    }
  }

  private Vector3 GetNewSpawnPosition() =>
      transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(0, maxSpawnDistance);
}
