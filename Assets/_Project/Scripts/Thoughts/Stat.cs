using System;
using UnityEngine;

public enum StatName {
  // модфифицируемые статы:
  Cooldown,
  Damage,
  ChainCount,
  ChainSpeedAddition,
  SpecialDamage,
  MaxHealth,
  DodgeChance,
  MoveSpeed,

  // динамические (=> немодифицируемые) статы:
  Health,
  Mana
}

[Serializable]
public class Stat {
  [SerializeField] private float _value;

  public float Value {
    get => _value;
    set => _value = value;
  }

  public void Change(StatOperationType type, float coefficient) {
    if (type == StatOperationType.Add) Value += coefficient;
    else if (type == StatOperationType.Multiply) Value *= coefficient;
  }
}
