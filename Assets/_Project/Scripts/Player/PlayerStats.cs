using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
  Health,
  MaxHealth,
  Money,
  MoveSpeed,
  Mana,
  Damage,
  // ...
}

public enum StatModifierType
{
  Add,
  Multiply
}

public class StatModifier
{
  public readonly float Value;
  public readonly StatModifierType Type;
  public readonly Effect SourceEffect;

  public StatModifier(float value, StatModifierType type, Effect sourceEffect)
  {
    Value = value;
    Type = type;
    SourceEffect = sourceEffect;
  }
}

public class PlayerStats : MonoBehaviour
{
  private readonly Dictionary<Stat, List<StatModifier>> _modifiers = new();

  int Money { get; }
  float MoveSpeed { get; }
  int Mana { get; }

  float GetStat(Stat stat) {
    return 0;
  }

  void AddModifier(Stat stat, StatModifier modifier) {}

  void RemoveModifier(Stat stat, StatModifier modifier) {}
}
