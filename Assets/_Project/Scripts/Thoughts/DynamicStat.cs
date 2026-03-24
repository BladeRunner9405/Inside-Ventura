using System;
using UnityEngine;

// динамические (=> немодифицируемые) статы:
public enum DynamicStatName {
  Health,
  Mana
}

[Serializable]
public class DynamicStat {
  [SerializeField] private float baseValue;

  public DynamicStat(float baseValue = 0f) {
    BaseValue = baseValue;
  }

  public float BaseValue {
    get => baseValue;
    set => baseValue = value;
  }

  public void Change(StatOperationType type, float value) {
    if (type == StatOperationType.Add)
      baseValue += value;
    else if (type == StatOperationType.Multiply) baseValue *= value;
  }
}
