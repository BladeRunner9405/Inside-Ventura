using System.Collections;
using CherryFramework.DependencyManager;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity {
  [Header("Enemy stuff")] public int damage;

  public bool isBoss;
  protected NavMeshAgent Agent;

  [Inject] protected PlayerAccessor PlayerAccessor;

  protected void Start() {
    Agent = GetComponent<NavMeshAgent>();

    // Otherwise the enemy gets teleported after being spawned.
    Invoke(nameof(EnableAI), 0.01F);

    Agent.updateRotation = false;
    Agent.updateUpAxis = false;
    Agent.speed = MoveSpeed;

    var player = PlayerAccessor.Player.transform;
    TargetTo(player);
  }

  private void Update() {
    if (Agent.enabled) Agent.SetDestination(target.position);
  }

  private void EnableAI() {
    Agent.enabled = true;
  }

  protected override IEnumerator DashCoroutine(Vector2 direction, float distance, float duration) {
    var originalAgentStatus = Agent.enabled;

    Agent.enabled = false;
    yield return base.DashCoroutine(direction, distance, duration);
    Agent.enabled = originalAgentStatus;
  }
}
