using UnityEngine;

public class PlayerAccessor : IPlayerData {
  private Player _player;

  public void RegisterPlayer(Player player) {
    _player = player;
  }

  public void UnregisterPlayer(Player player) {
    if (_player == player)
      _player = null;
  }

  public Transform Transform => _player?.transform;
  public PlayerEquipment Equipment => _player?.Equipment;
  public PlayerInventory Inventory => _player?.Inventory;
  public PlayerStats Stats => _player?.Stats;

  public Stat GetStat(StatName statName) {
    return _player?.GetStat(statName);
  }

  public void Dash(Vector2 direction, float distance, float duration)
  {
    _player?.Dash(direction, distance, duration);
  }

  public void TakeDamage(float amount) {
    _player?.TakeDamage(amount);
  }
}

public interface IPlayerData {
  Transform Transform { get; }
  PlayerEquipment Equipment { get; }
  PlayerStats Stats { get; }
  Stat GetStat(StatName statName);
  void Dash(Vector2 direction, float distance, float duration);
}
