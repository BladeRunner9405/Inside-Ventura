using System;
using CherryFramework.DependencyManager;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Entity : InjectMonoBehaviour {
  [SerializeField] private float health;
  [SerializeField] private ModifiableStat maxHealth = new(100f);
  [SerializeField] private bool isDead;

  [SerializeField] private bool isInvulnerable;

  [SerializeField] private ModifiableStat dodgeChance = new();

  public Transform target; // Transform, на кого смотрит Entity

  public float moveSpeed = 5f;

  protected Collider2D col;
  protected Rigidbody2D rb;

  public float Health {
    get => health;
    protected set => health = Mathf.Clamp(value, 0, MaxHealth);
  }

  public float MaxHealth => maxHealth.Value;

  public bool IsDead => isDead;

  public float DodgeChance => Mathf.Min(1f, dodgeChance.Value);

  public ModifiableStat GetStat(StatName statName) {
    if (statName == StatName.MaxHealth)
      return maxHealth;
    if (statName == StatName.DodgeChance)
      return dodgeChance;
    return null;
  }

  protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();

    health = MaxHealth;
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

    Debug.Log($"{gameObject.name} получил {finalAmount} урона. Его здоровье - {Health}/{maxHealth}");
  }

  public void Die() {
    if (IsDead) return;

    isDead = true;
    Health = 0;
    OnDeath?.Invoke();
  }

  public void TargetTo(Transform target) // назначить новую цель
  {
    this.target = target;
  }
}
