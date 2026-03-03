using UnityEngine;

public class Enemy : Entity
{
    public int damage;
    public bool isBoss;

    private void Awake() {

        Transform player = Object.FindFirstObjectByType<Player>().transform; // заглушка, врагам выдавать игрока будет, например, комната

        TargetTo(player);
    }

    private void Update() {
    if (target) {
            Move((target.position - transform.position).normalized);
        }
    }
}
