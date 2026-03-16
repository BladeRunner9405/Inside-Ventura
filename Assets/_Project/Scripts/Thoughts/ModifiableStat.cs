using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StatModifierType {
  Add,
  Multiply
}

public class StatModifier {
  public readonly Effect SourceEffect;
  public readonly StatModifierType Type;
  public readonly float Value;

  public StatModifier(float value, StatModifierType type, Effect sourceEffect) {
    Value = value;
    Type = type;
    SourceEffect = sourceEffect;
  }
}

[Serializable]
public class ModifiableStat {
  [SerializeField] private float baseValue;
  private List<StatModifier> modifiers = new();

  public ModifiableStat(float baseValue) {
    BaseValue = baseValue;
  }

  public float BaseValue {
    get => baseValue;
    set => baseValue = value;
  }

  public float Value {
    get {
      var addSum = modifiers.Where(m => m.Type == StatModifierType.Add).Sum(m => m.Value);
      var multiplyFactor = 1f + modifiers.Where(m => m.Type == StatModifierType.Multiply).Sum(m => m.Value);
      return (baseValue + addSum) * multiplyFactor;
    }
  }

  public void AddModifier(StatModifier modifier) {
    modifiers.Add(modifier);
  }

  public void RemoveModifier(StatModifier modifier) {
    modifiers.Remove(modifier);
  }

  public void RemoveAllEffectModifiers(Effect effect) {
    modifiers.RemoveAll(m => m.SourceEffect == effect);
  }
}
