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

public class Stat
{
  [SerializeField] protected float value;

  public Stat(float value = 0f) {
    Value = value;
  }

  public float Value {
    get => value;
    set => this.value = value;
  }
}
