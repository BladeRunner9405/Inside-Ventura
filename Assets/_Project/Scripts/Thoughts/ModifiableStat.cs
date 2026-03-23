using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// модфифицируемые статы:
public enum ModifiableStatName {
  Cooldown,
  Damage,
  ChainCount,
  ChainSpeedAddition,
  SpecialDamage,
  MaxHealth,
  DodgeChance,
  MoveSpeed
}

public enum StatOperationType {
  Add,
  Multiply
}

public class StatModifier {
  public readonly StatOperationType Type;
  public readonly float Value;

  public StatModifier(StatOperationType type, float value) {
    Type = type;
    Value = value;
  }
}

[Serializable]
public class ModifiableStat {
  [SerializeField] private float baseValue;
  private List<StatModifier> modifiers = new();

  public ModifiableStat(float baseValue = 0f) {
    BaseValue = baseValue;
  }

  public float BaseValue {
    get => baseValue;
    set => baseValue = value;
  }

  public float Value {
    get {
      var addSum = modifiers.Where(m => m.Type == StatOperationType.Add).Sum(m => m.Value);
      var multiplyFactor = modifiers.Where(m => m.Type == StatOperationType.Multiply)
        .Aggregate(1f, (current, m) => current * m.Value);
      return (baseValue + addSum) * multiplyFactor;
    }
  }

  public void AddModifier(StatModifier modifier) {
    modifiers.Add(modifier);
  }

  public void RemoveModifier(StatModifier modifier) {
    modifiers.Remove(modifier);
  }
}
