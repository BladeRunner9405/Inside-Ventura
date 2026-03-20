using System;
using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class Entity : InjectMonoBehaviour {
  [SerializeField] private float health;
  [SerializeField] private float maxHealth = 100;
  [SerializeField] private bool isDead;

  [SerializeField] private bool isInvulnerable;

  public Transform target; // Transform, на кого смотрит Entity

  public float moveSpeed = 5f;

  protected Collider2D col;
  protected Rigidbody2D rb;

  public float Health {
    get => health;
    protected set => health = Mathf.Clamp(value, 0, MaxHealth);
  }

  public float MaxHealth {
    get => maxHealth;
    protected set {
      maxHealth = Mathf.Max(1, value);
      if (health > maxHealth)
        health = maxHealth;
    }
  }

  public bool IsDead => isDead;

  protected virtual void Awake() {
    rb = GetComponent<Rigidbody2D>();
    col = GetComponent<Collider2D>();

    health = maxHealth;
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

    Health -= amount;

    OnTakeDamage?.Invoke(amount);

    if (Health == 0)
      Die();

    Debug.Log($"{gameObject.name} получил {amount} урона. Его здоровье - {Health}/{maxHealth}");
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
