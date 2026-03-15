using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class UncontrollableRage : Enemy {
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
  [SerializeField] private List<BoxCollider2D> hitboxes = new();
  [SerializeField] private float attackWaveDuration = 0.5F;


  [Header("Advanced"), SerializeField] private float retreatLookupDistance = 0.2f;

  private Transform _hitboxParent;
  private State _state = State.Walking;
  private float _curCooldown;
  private int _curHitbox = -1;
  private Animator _animator;

  protected new void Start() {
    base.Start();

    _animator = GetComponent<Animator>();
    _hitboxParent = hitboxes[0].transform.parent;
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
    }

    // Transform attack wave to look at player.
    if (_state != State.Attacking) {
      var diff = target.position - transform.position;
      diff.Normalize();
      var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
      _hitboxParent.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);
    }
  }

  private void TryAttack() {
    var dist = Vector3.Distance(target.position, transform.position);
    if (dist > playerDistance || _curCooldown > 0.0F) return;

    Agent.isStopped = true;
    _state = State.Attacking;
    _animator.SetTrigger("StartAttacking");
  }

  // ReSharper disable once InconsistentNaming
  // Called from UA animation clip.
  private void UADoDamage() {
    _curHitbox = 0;
    DOTween.Sequence().AppendCallback(() => { Debug.Log("Wave sequence started"); })
      .Append(DOTween.To(() => _curHitbox, UpdateHitbox, 3, attackWaveDuration))
      .AppendCallback(() => {
        _curHitbox = -1;
        Debug.Log("Wave sequence ended");
      }).PlayForward();
  }

  private void UpdateHitbox(int newVal) {
    if (newVal == _curHitbox) {
      return;
    }

    Debug.Log($"Updating hitbox with new index {newVal}");
    _curHitbox = newVal;
    if (hitboxes[_curHitbox].OverlapPoint(target.position)) {
      PlayerAccessor.Player.TakeDamage(damage);
    }
  }

  // ReSharper disable once InconsistentNaming
  // Called from UA animation clip.
  private void UAStartCooldown() {
    _curCooldown = cooldownDuration;
    Agent.isStopped = false;
    ResolveWalkingDirection();
  }

  private void ResolveWalkingDirection() {
    var dist = Vector3.Distance(target.position, transform.position);
    _state = dist > playerDistance ? State.Walking : dist < retreatDistance ? State.Retreating : State.Idle;
  }


  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.white;
    Gizmos.DrawWireSphere(transform.position, playerDistance);

    Gizmos.color = Color.yellow;
    var dir = (transform.position - target.position).normalized;
    Gizmos.DrawLine(transform.position, transform.position + dir * retreatLookupDistance * 5);

    Gizmos.DrawWireSphere(transform.position, retreatDistance);
  }

  private void OnDrawGizmos() {
    if (_curCooldown > 0.0F) {
      UnityEditor.Handles.Label(transform.position, $"Cooldown:  {_curCooldown}\nState: {_state}");
    }

    if (_curHitbox != -1) {
      var curCollider = hitboxes[_curHitbox];
      Gizmos.matrix = Matrix4x4.TRS(_hitboxParent.transform.position, _hitboxParent.transform.rotation, Vector3.one);
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(curCollider.transform.localPosition, new Vector3(hitboxWidth, hitboxWidth, 0));
    }
  }
}
