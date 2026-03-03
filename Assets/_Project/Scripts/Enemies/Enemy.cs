using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Entity
{
    public int damage;
    public bool isBoss;

    protected override void Awake() {
        base.Awake();

        Transform player = Object.FindFirstObjectByType<Player>().transform; // заглушка, врагам выдавать игрока будет, например, комната

        TargetTo(player);
    }

    private void FixedUpdate() {
    if (target) {
            Move((target.position - transform.position).normalized);
        }
    }
}
