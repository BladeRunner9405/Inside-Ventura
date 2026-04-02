using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;

public class Player : Entity
{
  [Header("Player stuff")]
  [SerializeField]
  private ItemPickup itemPickup;

  [SerializeField]
  private PlayerInventory inventory;

  [SerializeField]
  private PlayerEquipment equipment;

  [SerializeField]
  private PlayerStats stats;

  [Header("Invulnerability")]
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
    OnTakeDamage -= HandleTakeDamage;

    _playerAccessor.UnregisterPlayer(this);

    OnTakeDamage -= TriggerCameraShake;
  }

  // 1. Вычисляем Максимальное Здоровье динамически
  public override float MaxHealth
  {
    get
    {
      // Базовое здоровье (из инспектора Entity) + Бонусы от Сердца
      float heartBonus =
        Equipment?.Heart != null ? Equipment.Heart.MaxHealthBonus.ModifiedValue : 0f;
      return base.MaxHealth + heartBonus;
    }
  }

  // 2. Связываем текущее здоровье с системой сохранений (PlayerStats)
  public override float Health
  {
    get
    {
      if (Stats == null)
        return base.Health; // Защита на старте
      return Stats.CurrentHealth;
    }
    set
    {
      if (Stats == null)
      {
        base.Health = value;
        return;
      }
      // Сохраняем в модель, не давая превысить новый Максимум!
      Stats.CurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }
  }

  public void TryToInteract()
  {
    itemPickup.TryToInteract();
  }

  private void TriggerCameraShake(float damageAmount)
  {
    // Можно привязать силу тряски к размеру полученного урона
    CameraShaker.Instance.ShakeCamera(damageAmount);
  }

  private void HandleTakeDamage(float finalAmount)
  {
    if (finalAmount > 0 && !_invulnerabilityRunning)
      StartCoroutine(InvulnerabilityCoroutine());
  }

  private IEnumerator InvulnerabilityCoroutine()
  {
    _invulnerabilityRunning = true;
    ++InvulnerabilityProcCount;

    var elapsed = 0f;
    while (elapsed < invulnerabilityDuration)
    {
      elapsed += Time.deltaTime;
      yield return null;
    }

    --InvulnerabilityProcCount;
    _invulnerabilityRunning = false;
  }

  protected override IEnumerator DashCoroutine(Vector2 direction, float distance, float duration)
  {
    ++InvulnerabilityProcCount;
    yield return base.DashCoroutine(direction, distance, duration);
    --InvulnerabilityProcCount;
  }
}
