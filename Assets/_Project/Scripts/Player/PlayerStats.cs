using UnityEngine;

public class PlayerStats : MonoBehaviour {
  [SerializeField] private ModifiableStat money; // Замыслы, игровая валюта
  [SerializeField] private ModifiableStat mana; // Idea Points, тратятся активацией активируемых мыслей

  public float Money => money.Value;
  public float Mana => mana.Value;
}
