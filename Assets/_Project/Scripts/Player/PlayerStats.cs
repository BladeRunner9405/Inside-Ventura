using UnityEngine;

public class PlayerStats : MonoBehaviour {
  [SerializeField] private DynamicStat money = new(); // Замыслы, игровая валюта
  [SerializeField] private DynamicStat mana = new(); // Idea Points, тратятся активацией активируемых мыслей

  public float Money {
    get => money.BaseValue;
    set => money.BaseValue = value;
  }
  public float Mana {
    get => mana.BaseValue;
    set => mana.BaseValue = value;
  }

  public ModifiableStat GetStat(ModifiableStatName statName) {
    return null;
  }

  public DynamicStat GetStat(DynamicStatName statName) {
    if (statName == DynamicStatName.Mana)
      return mana;
    return null;
  }
}
