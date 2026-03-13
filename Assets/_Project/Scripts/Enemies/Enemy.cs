using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity {
  public int damage;
  public bool isBoss;

  [Inject] private PlayerAccessor _playerAccessor;
  private NavMeshAgent _agent;

  protected void Start() {
    var player = _playerAccessor.Player.transform;

    _agent = GetComponent<NavMeshAgent>();

    // Otherwise the enemy gets teleported after being spawned.
    Invoke(nameof(EnableAI), 0.01F);

    _agent.updateRotation = false;
    _agent.updateUpAxis = false;

    TargetTo(player);
  }

  private void Update() {
    if (_agent.enabled) {
      _agent.SetDestination(target.position);
    }
  }

  private void EnableAI() {
    _agent.enabled = true;
  }
}
