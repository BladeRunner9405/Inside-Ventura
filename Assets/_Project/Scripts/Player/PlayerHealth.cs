using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
  int Health { get; }
  int MaxHealth { get; }

  event Action<int> OnTakeDamage;
  event Action OnDeath;

  void TakeDamage(int amount) {}

  void Heal(int amount) {}

  void Die() {}

  void ModifyMaxHealth(int delta) {}
}
