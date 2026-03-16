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

  private int Money { get; } // Замыслы, игровая валюта
  private float MoveSpeed { get; }
  private int Mana { get; } // Idea Points, тратятся активацией активируемых мыслей

  private float GetStat(Stat stat) {
    return 0;
  }

  private void AddModifier(Stat stat, StatModifier modifier) {
  }

  private void RemoveModifier(Stat stat, StatModifier modifier) {
  }
}
