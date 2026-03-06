using System.Collections.Generic;
using UnityEngine;

public enum Stat {
  Money,
  MoveSpeed,

  Mana
  // ...
}

public class PlayerStats : MonoBehaviour {
  private readonly Dictionary<Stat, List<StatModifier>> _modifiers = new();

  private int Money { get; }
  private float MoveSpeed { get; }
  private int Mana { get; }

  private float GetStat(Stat stat) {
    return 0;
  }

  private void AddModifier(Stat stat, StatModifier modifier) {
  }

  private void RemoveModifier(Stat stat, StatModifier modifier) {
  }
}
