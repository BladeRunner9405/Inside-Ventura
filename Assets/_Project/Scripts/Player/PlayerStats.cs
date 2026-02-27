using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
  private readonly Dictionary<Stat, List<StatModifier>> _modifiers = new();

  int Money { get; }
  float MoveSpeed { get; }
  int Mana { get; }

  float GetStat(Stat stat) {}

  void AddModifier(Stat stat, StatModifier modifier) {}

  void RemoveModifier(Stat stat, StatModifier modifier) {}
}
