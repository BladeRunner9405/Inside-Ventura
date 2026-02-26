using System;
using UnityEngine;

public interface IPlayer
{
  int Health { get; }
  int MaxHealth { get; }
  float MoveSpeed { get; }
  int Money { get; set; }

  Weapon Weapon { get; }
  Heart Heart { get; }
  Accessory Accessory { get; }
  ThoughtBag ThoughtBag { get; }

  event Action<int> OnTakeDamage;
  event Action OnDeath;
  event Action OnRoomCleared;

  void TakeDamage(int amount);
  void Attack();
  void UseAbility();
  void Die();
  void ModifyMaxHealth(int delta);
}
