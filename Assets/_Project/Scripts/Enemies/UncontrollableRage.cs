using System.Collections.Generic;
using CherryFramework.DependencyManager;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class UncontrollableRage : Entity {
  private enum State {
    Walking,
    Attacking,
    Retreating,
    Idle
  }

  public int damage;
  public bool isBoss;

  [SerializeField] private float playerDistance = 1.5F;
  [SerializeField] private float retreatDistance = 1F;
  [SerializeField] private float cooldownDuration = 0.3F;

  [Header("Attack"), SerializeField] private float hitboxWidth = 1F;
  [SerializeField] private float hitboxesHeight = 1.2F;
  [SerializeField] private List<BoxCollider2D> hitboxes = new List<BoxCollider2D>();
  [SerializeField] private float attackWaveDuration = 0.5F;


  [Header("Advanced"), SerializeField] private float retreatLookupDistance = 0.2f;

  [Inject] private PlayerAccessor _playerAccessor;
  private Transform _hitboxParent;
  private NavMeshAgent _agent;
  private State _state = State.Walking;
  private float _curCooldown = 0.0F;
  private int _curHitbox = -1;
  private Animator _animator;

  protected void Start() {
    var player = _playerAccessor.Player.transform;

    _agent = GetComponent<NavMeshAgent>();
    _animator = GetComponent<Animator>();
    _hitboxParent = hitboxes[0].transform.parent;

    // Otherwise the enemy gets teleported after being spawned.
    Invoke(nameof(EnableAI), 0.01F);

    _agent.updateRotation = false;
    _agent.updateUpAxis = false;
    _agent.speed = moveSpeed;

    TargetTo(player);
  }

  private void Update() {
    if (!_agent.enabled) return;
    _curCooldown -= Time.deltaTime;

    switch (_state) {
      case State.Walking:
        _agent.isStopped = false;
        _agent.SetDestination(target.position);
        UpdateWalkingDirection();
        TryAttack();
        break;

      case State.Idle:
        _agent.isStopped = true;
        UpdateWalkingDirection();
        TryAttack();
        break;

      case State.Retreating:
        _agent.isStopped = false;
        var dir = (transform.position - target.position).normalized;
        _agent.SetDestination(transform.position + dir * retreatLookupDistance);
        UpdateWalkingDirection();
        TryAttack();
        break;
    }

    if (_state != State.Attacking) {
      // _hitboxParent.LookAt(target, new Vector3(0, 0, -1));
      var diff = target.position - transform.position;
      diff.Normalize();
      var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
      _hitboxParent.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);
    }
  }

  private void TryAttack() {
    var dist = Vector3.Distance(target.position, transform.position);
    if (dist > playerDistance || _curCooldown > 0.0F) return;

    _agent.isStopped = true;
    _state = State.Attacking;
    _animator.SetTrigger("StartAttacking");
  }

  private void EnableAI() {
    _agent.enabled = true;
  }

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
      _playerAccessor.Player.TakeDamage(damage);
    }
  }

  private void UAStartCooldown() {
    _curCooldown = cooldownDuration;
    _agent.isStopped = false;
    UpdateWalkingDirection();
  }

  private void UpdateWalkingDirection() {
    var dist = Vector3.Distance(target.position, transform.position);
    _state = dist > playerDistance ? State.Walking : dist < retreatDistance ? State.Retreating :  State.Idle;
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
