using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class UncontrollableAnger : Enemy {
  private enum State {
    Walking,
    Attacking,
    Retreating,
    Idle
  }

  [SerializeField] private float playerDistance = 2.7F;
  [SerializeField] private float retreatDistance = 2F;
  [SerializeField] private float cooldownDuration = 3F;

  [Header("Attack"), SerializeField] private float hitboxWidth = 1F;
  [SerializeField] private float hitboxesHeight = 1.2F;
  [SerializeField] private GameObject attackField;

  [Header("Advanced"), SerializeField, Tooltip("Distance that enemy uses to find a place to retreat")]
  private float retreatLookupDistance = 0.2f;

  private State _state = State.Walking;
  private float _curCooldown;

  private Animator _animator;

  protected new void Start() {
    base.Start();
    _animator = GetComponent<Animator>();
  }

  private void Update() {
    if (!Agent.enabled) return;
    _curCooldown -= Time.deltaTime;

    switch (_state) {
      case State.Walking:
        Agent.isStopped = false;
        Agent.SetDestination(target.position);
        ResolveWalkingDirection();
        TryAttack();
        break;

      case State.Idle:
        Agent.isStopped = true;
        ResolveWalkingDirection();
        TryAttack();
        break;

      case State.Retreating:
        Agent.isStopped = false;
        var dir = (transform.position - target.position).normalized;
        Agent.SetDestination(transform.position + dir * retreatLookupDistance);
        ResolveWalkingDirection();
        TryAttack();
        break;
      case State.Attacking:
        Agent.SetDestination(transform.position);
        Agent.isStopped = true;
        break;
    }
  }

  private void TryAttack() {
    var dist = Vector3.Distance(target.position, transform.position);
    if (dist > playerDistance || _curCooldown > 0.0F) return;

    _state = State.Attacking;
    _animator.SetTrigger("StartAttacking");
  }

  // ReSharper disable once InconsistentNaming
  // Called from UA animation clip. Spawn the attack field.
  private void UADoDamage() {
    var diff = target.position - transform.position;
    diff.Normalize();
    var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    var rot = Quaternion.Euler(0f, 0f, rotZ + 90);

    Instantiate(attackField, transform.position, rot).GetComponent<UAAttackField>().damage = damage;
  }

  // ReSharper disable once InconsistentNaming
  // Called from UA animation clip.
  private void UAStartCooldown() {
    _curCooldown = cooldownDuration;
    ResolveWalkingDirection();
  }

  private void ResolveWalkingDirection() {
    var dist = Vector3.Distance(target.position, transform.position);
    _state = dist > playerDistance ? State.Walking : dist < retreatDistance ? State.Retreating : State.Idle;
  }


  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.white;
    Gizmos.DrawWireSphere(transform.position, playerDistance);

    if (target != null) {
      Gizmos.color = Color.yellow;
      var dir = (transform.position - target.position).normalized;
      Gizmos.DrawLine(transform.position, transform.position + dir * retreatLookupDistance * 5);
    }

    Gizmos.DrawWireSphere(transform.position, retreatDistance);
  }

  private void OnDrawGizmos() {
    if (_curCooldown > 0.0F) {
      UnityEditor.Handles.Label(transform.position, $"Cooldown:  {_curCooldown}\nState: {_state}");
    }
  }
}
