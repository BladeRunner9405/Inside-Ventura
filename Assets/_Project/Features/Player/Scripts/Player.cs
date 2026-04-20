using System;
using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;

public class Player : Entity
{
  [Header("Components")]
  [SerializeField]
  private ItemPickup itemPickup;

  [SerializeField]
  private PlayerInventory inventory;

  [SerializeField]
  private PlayerEquipment equipment;

  [SerializeField]
  private PlayerStats stats;

  [SerializeField]
  private AimTarget playerAim; // Перенесено из контроллера

  [Header("Invulnerability Settings")]
  [SerializeField]
  private float invulnerabilityDuration = 0.5f;

  private bool _invulnerabilityRunning;

  [Inject]
  private PlayerAccessor _playerAccessor;

  public PlayerInventory Inventory => inventory;
  public PlayerEquipment Equipment => equipment;
  public PlayerStats Stats => stats;

  protected override void OnEnable()
  {
    base.OnEnable();
    _playerAccessor.RegisterPlayer(this);

    OnTakeDamage += TriggerCameraShake;
    OnTakeDamage += HandleTakeDamage;
  }

  private void OnDisable()
  {
    _playerAccessor.UnregisterPlayer(this);
    OnTakeDamage -= TriggerCameraShake;
    OnTakeDamage -= HandleTakeDamage;
  }

  #region Health & Stats Logic

  /*public override float MaxHealth
  {
    get {
      // Динамический расчет: База + Бонус от экипированного сердца
      float heartBonus =
        (Equipment != null && Equipment.Heart != null)
          ? Equipment.Heart.MaxHealthBonus.ModifiedValue
          : 0f;
      return base.MaxHealth + heartBonus;
    }
  }*/

  public override float Health
  {
    get => Stats != null ? Stats.CurrentHealth.Value : base.Health;
    set
    {
      if (Stats == null)
      {
        base.Health = value;
        return;
      }
      // Ограничиваем здоровье текущим динамическим максимумом
      Stats.CurrentHealth.Value = Mathf.Clamp(value, 0, MaxHealth);
    }
  }

  public override Stat GetStat(StatName statName) {
    switch (statName)
    {
      case StatName.Health: return Stats.CurrentHealth;
      default: return base.GetStat(statName);
    }
  }

  #endregion

  #region Facade Methods (Команды для контроллера)

  public void TryToInteract()
  {
    if (itemPickup != null)
      itemPickup.TryToInteract();
  }

  public override void Attack(Vector2 direction)
  {
    if (Equipment != null)
    {
      Equipment.TryToAttack(direction);
    }
  }

  public void UseAbility(Vector2 direction)
  {
    if (Equipment != null)
    {
      Equipment.TryToUseAbility(direction);
    }
  }

  public void LookAt(Vector2 targetWorldPosition)
  {
    if (playerAim != null)
    {
      playerAim.aimAt(targetWorldPosition);
    }
  }

  #endregion

  #region Damage & Effects

  private void TriggerCameraShake(float damageAmount)
  {
    if (CameraShaker.Instance != null)
      CameraShaker.Instance.ShakeCamera(damageAmount);
  }

  private void HandleTakeDamage(float finalAmount)
  {
    if (finalAmount > 0 && !_invulnerabilityRunning && gameObject.activeInHierarchy)
      StartCoroutine(InvulnerabilityCoroutine());
  }

  private IEnumerator InvulnerabilityCoroutine()
  {
    _invulnerabilityRunning = true;
    ++InvulnerabilityProcCount;

    yield return new WaitForSeconds(invulnerabilityDuration);

    --InvulnerabilityProcCount;
    _invulnerabilityRunning = false;
  }

  protected override IEnumerator DashCoroutine(Vector2 direction, float distance, float duration)
  {
    ++InvulnerabilityProcCount;
    yield return base.DashCoroutine(direction, distance, duration);
    --InvulnerabilityProcCount;
  }

  #endregion
}
