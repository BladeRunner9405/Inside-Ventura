using System;
using UnityEngine;

public class PlayerAccessor : IPlayerData
{
  private Player _player;
  public Transform Transform => _player?.transform;

  public PlayerInventory Inventory => _player?.Inventory;
  public PlayerEquipment Equipment => _player?.Equipment;
  public PlayerStats Stats => _player?.Stats;

  public bool HasPlayer => _player != null;
  public event Action OnPlayerRegistered;
  public event Action OnPlayerUnregistered;
  public event Action<StatName> OnStatModified;

  public void RegisterPlayer(Player player)
  {
    _player = player;
    OnPlayerRegistered?.Invoke();
  }

  public void UnregisterPlayer(Player player)
  {
    if (_player == player)
      _player = null;
    OnPlayerUnregistered?.Invoke();
  }

  public Stat GetStat(StatName statName)
  {
    var stat = _player?.GetStat(statName);
    if (stat != null) return stat;

    if (Stats) {
      stat = Stats.GetStat(statName);
      if (stat != null) return stat;
    }

    if (Equipment) {
      stat = Equipment.Accessory.GetStat(statName);
      if (stat != null) return stat;
      stat = Equipment.Heart.GetStat(statName);
      if (stat != null) return stat;
      stat = Equipment.Weapon.GetStat(statName);
      if (stat != null) return stat;
    }

    return null;
  }

  public float GetStatValue(StatName statName) {
    if (GetStat(statName) is ModifiableStat modifiableStat) return modifiableStat.ModifiedValue;
    if (GetStat(statName) is DataModelStat dataModelStat) return dataModelStat.Value;
    return GetStat(statName).Value;
  }

  public void NotifyStatModified(StatName statName)
  {
    OnStatModified?.Invoke(statName);
  }

  public void Dash(Vector2 direction, float distance, float duration)
  {
    _player?.Dash(direction, distance, duration);
  }

  public void TakeDamage(float amount)
  {
    _player?.TakeDamage(amount);
  }
}

public interface IPlayerData
{
  Transform Transform { get; }
  PlayerInventory Inventory { get; }
  PlayerEquipment Equipment { get; }
  PlayerStats Stats { get; }
  Stat GetStat(StatName statName);
  void Dash(Vector2 direction, float distance, float duration);
  void TakeDamage(float amount);
}
