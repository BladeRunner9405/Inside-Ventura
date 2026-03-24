using UnityEngine;

public class PlayerStats : MonoBehaviour {
  [SerializeField] private DynamicStat money = new(); // Замыслы, игровая валюта
  [SerializeField] private DynamicStat mana = new(); // Idea Points, тратятся активацией активируемых мыслей

  public float Money {
    get => money.Value;
    set => money.Value = value;
  }

  public float Mana {
    get => mana.Value;
    set => mana.Value = value;
  }

  public Stat GetStat(StatName statName) {
    if (statName == StatName.Mana)
      return mana;
    return null;
  }
}
