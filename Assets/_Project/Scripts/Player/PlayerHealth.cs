using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
  private int _health;
  private int _maxHealth;
  private bool _isDead;

  public int Health
  {
    get => _health;
    private set => _health = Mathf.Clamp(value, 0, MaxHealth);
  }

  public int MaxHealth
  {
    get => _maxHealth;
    private set
    {
      _maxHealth = Mathf.Max(1, value);
      if (_health > _maxHealth)
        _health = _maxHealth;
    }
  }

  event Action<int> OnTakeDamage;
  event Action OnDeath;

  private void Awake()
  {
    _maxHealth = 100;
    _health = _maxHealth;
    _isDead = false;
  }

  void TakeDamage(int amount) {
    if (_isDead) return;
    if (amount <= 0) return;

    Health -= amount;

    OnTakeDamage?.Invoke(amount);

    if (Health == 0)
      Die();
  }

  void Heal(int amount) {
    if (_isDead) return;
    if (amount <= 0) return;

    Health += amount;
  }

  void Die() {
    if (_isDead) return;

    _isDead = true;
    Health = 0;
    OnDeath?.Invoke();
  }

  void ModifyMaxHealth(int delta) {
    if (_isDead) return;

    MaxHealth += delta;
  }
}
