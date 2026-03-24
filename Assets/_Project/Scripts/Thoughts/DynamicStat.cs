using System;
using UnityEngine;

[Serializable]
public class DynamicStat : Stat {
  public DynamicStat(float value = 0f) {
    Value = value;
  }

  public void Change(StatOperationType type, float coefficient) {
    if (type == StatOperationType.Add) value += coefficient;
    else if (type == StatOperationType.Multiply) value *= coefficient;
  }
}
