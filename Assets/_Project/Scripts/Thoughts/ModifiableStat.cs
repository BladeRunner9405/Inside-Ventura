using System;
using System.Collections.Generic;
using System.Linq;

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
public class ModifiableStat : Stat {
  private List<StatModifier> modifiers = new();

  public ModifiableStat(float value = 0f) {
    Value = value;
  }

  public float ModifiedValue {
    get {
      var addSum = modifiers.Where(m => m.Type == StatOperationType.Add).Sum(m => m.Value);
      var multiplyFactor = modifiers.Where(m => m.Type == StatOperationType.Multiply)
        .Aggregate(1f, (current, m) => current * m.Value);
      return (value + addSum) * multiplyFactor;
    }
  }

  public void AddModifier(StatModifier modifier) {
    modifiers.Add(modifier);
  }

  public void RemoveModifier(StatModifier modifier) {
    modifiers.Remove(modifier);
  }
}
