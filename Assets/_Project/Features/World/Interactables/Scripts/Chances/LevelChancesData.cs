using System;
using InsideVentura.World;
using UnityEngine;

[Serializable]
public class RoomChancesData
{
  public ItemChancesData Money;
  // public StatChances Mana;
  public ItemChancesData Health;
  public ItemChancesData Thoughts;

  public RoomChancesData(ItemChancesData money, ItemChancesData health, ItemChancesData thoughts)
  {
    Money = money ?? ItemChancesData.Never;
    Health = health ?? ItemChancesData.Never;
    Thoughts = thoughts ?? ItemChancesData.Never;
  }
}

[CreateAssetMenu(fileName = "NewLevelChances", menuName = "InsideVentura/Chances/LevelChances")]
public class LevelChancesData : ScriptableObject
{
  [SerializeField] private RoomChancesData SafeRoomChances;
  [SerializeField] private RoomChancesData NormalRank1Chances;
  [SerializeField] private RoomChancesData NormalRank2Chances;
  [SerializeField] private RoomChancesData NormalRank3Chances;
  [SerializeField] private RoomChancesData MiniBossChances;
  [SerializeField] private RoomChancesData BossChances;

  private RoomChancesData _defaultChances;

  public LevelChancesData()
  {
    _defaultChances = NormalRank1Chances;
  }

  public RoomChancesData GetCurrentChances()
  {
    RoomType currentType = DungeonManager.Instance.GetCurrentRoomType();
    RoomChancesData selected = currentType switch
    {
      RoomType.Safe => SafeRoomChances,
      RoomType.NormalRank1 => NormalRank1Chances,
      RoomType.NormalRank2 => NormalRank2Chances,
      RoomType.NormalRank3 => NormalRank3Chances,
      RoomType.MiniBoss => MiniBossChances,
      RoomType.Boss => BossChances,
      _ => _defaultChances
    };

    if (selected == null)
    {
      selected = _defaultChances;
    }

    return selected;
  }
}
