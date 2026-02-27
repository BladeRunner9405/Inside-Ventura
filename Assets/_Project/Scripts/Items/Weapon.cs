using UnityEngine;

public abstract class Weapon : Artifact {
  [SerializeField] private float attackSpeed = 1f;
  [SerializeField] private float damage = 10f;

  public abstract void Attack();
}
