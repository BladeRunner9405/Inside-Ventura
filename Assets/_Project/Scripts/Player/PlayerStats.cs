using System.Collections.Generic;
using UnityEngine;

public enum Stat {
  Money,
  MoveSpeed,
  Mana,
  // ...
}

public class PlayerStats : MonoBehaviour {
  private readonly Dictionary<Stat, List<StatModifier>> _modifiers = new();

  int Money { get; }
  float MoveSpeed { get; }
  int Mana { get; }

  float GetStat(Stat stat) {
    return 0;
  }

  void AddModifier(Stat stat, StatModifier modifier) {
  }

  void RemoveModifier(Stat stat, StatModifier modifier) {
  }
}
