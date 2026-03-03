using UnityEngine;

public abstract class Entity: MonoBehaviour
{
    public int MaxHealth;
    public float MoveSpeed;
    public Transform target; // Transform, на кого смотрит Entity

    private int Health;

    public void Move(Vector3 dir) // переместиться в направлении dir
    {
        dir *= Time.deltaTime * MoveSpeed;
        transform.localPosition = new Vector3(transform.position.x + dir.x, 
                                         transform.position.y + dir.y,  
                                         transform.position.z);
    }

    public void Attack(Player player) {
        // ...
    }

    public void TakeDamage(int amount) {
        // ...
    }

    public void Die() {
        // ...
    }

    public void TargetTo(Transform target) // назначить новую цель
    {
        this.target = target;
    }
}
