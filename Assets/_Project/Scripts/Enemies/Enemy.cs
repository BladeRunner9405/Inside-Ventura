using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity {
  public int damage;
  public bool isBoss;

  [Inject] protected PlayerAccessor PlayerAccessor;
  protected NavMeshAgent Agent;

  protected void Start() {
    Agent = GetComponent<NavMeshAgent>();

    // Otherwise the enemy gets teleported after being spawned.
    Invoke(nameof(EnableAI), 0.01F);

    Agent.updateRotation = false;
    Agent.updateUpAxis = false;
    Agent.speed = moveSpeed;

    var player = PlayerAccessor.Player.transform;
    TargetTo(player);
  }

  private void Update() {
    if (Agent.enabled) {
      Agent.SetDestination(target.position);
    }
  }

  private void EnableAI() {
    Agent.enabled = true;
  }
}
