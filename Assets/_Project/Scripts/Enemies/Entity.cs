using System;
using CherryFramework.DependencyManager;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Entity : InjectMonoBehaviour {
  [SerializeField] private DynamicStat health = new();
  [SerializeField] private ModifiableStat maxHealth = new(100f);
  [SerializeField] private bool isDead;

  [SerializeField] private bool isInvulnerable;

  [SerializeField] private ModifiableStat dodgeChance = new();

  public Transform target; // Transform, на кого смотрит Entity

  [SerializeField] private ModifiableStat moveSpeed = new(5f);

  protected Collider2D col;
  protected Rigidbody2D rb;

  public float Health {
    get => health.BaseValue;
    set => health.BaseValue = value;
  }

  public float MaxHealth => maxHealth.Value;

  public bool IsDead => isDead;

  public float DodgeChance => Mathf.Min(1f, dodgeChance.Value);

  public float MoveSpeed {
    get => moveSpeed.Value;
    set => moveSpeed.BaseValue = value;
  }

  public ModifiableStat GetStat(ModifiableStatName statName) {
    if (statName == ModifiableStatName.MaxHealth)
      return maxHealth;
    if (statName == ModifiableStatName.DodgeChance)
      return dodgeChance;
    if (statName == ModifiableStatName.MoveSpeed)
      return moveSpeed;
    return null;
  }

  public DynamicStat GetStat(DynamicStatName statName) {
    if (statName == DynamicStatName.Health)
      return health;
    return null;
  }

  protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();

    Health = MaxHealth;
    isDead = false;
  }

  public void SetInvulnerable(bool invulnerable) {
    isInvulnerable = invulnerable;
  }

  private event Action<float> OnTakeDamage;
  public event Action OnDeath;

  public void TakeDamage(float amount) {
    if (IsDead || isInvulnerable) return;
    if (amount <= 0) return;

    var hasDodged = Random.value <= DodgeChance;
    var finalAmount = hasDodged ? 0f : amount;

    Health -= finalAmount;

    OnTakeDamage?.Invoke(finalAmount);

    if (Health == 0)
      Die();

    Debug.Log($"{gameObject.name} получил {finalAmount} урона. Его здоровье - {Health}/{MaxHealth}");
  }

  public void Die() {
    if (IsDead) return;

    GetComponent<SpriteRenderer>().enabled = false; // заглушка

    isDead = true;
    Health = 0;
    OnDeath?.Invoke();
  }

  public void TargetTo(Transform target) // назначить новую цель
  {
    this.target = target;
  }
}
