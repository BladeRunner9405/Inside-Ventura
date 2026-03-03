using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int MaxHealth;
    public Transform target; // Transform, на кого смотрит Entity

    public float moveSpeed = 5f;
    protected Rigidbody2D rb;
    protected Collider2D col;

    private ContactFilter2D _contactFilter; 
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private const float _shellDistance = 0.01f; // отступ, чтобы не врастать в стены
    

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(LayerMask.GetMask("Obstacle")); 
        _contactFilter.useLayerMask = true;
    }

    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        Vector2 deltaMove = direction * moveSpeed * Time.fixedDeltaTime;

        ResolveOverlap(); // проверка уже внутри стены

        int maxIterations = 4; 
        for (int i = 0; i < maxIterations; i++) {
            float distance = deltaMove.magnitude;
            if (distance < 0.0001f) break;

            int count = col.Cast(deltaMove.normalized, _contactFilter, _hitBuffer, distance + _shellDistance);

            if (count > 0) {
                RaycastHit2D hit = _hitBuffer[0];
                
                float safeDistance = Mathf.Max(0, hit.distance - _shellDistance);
                rb.position += deltaMove.normalized * safeDistance;

                Vector2 remainingDelta = deltaMove.normalized * (distance - safeDistance);
                deltaMove = remainingDelta - Vector2.Dot(remainingDelta, hit.normal) * hit.normal;

                if (Vector2.Dot(deltaMove, direction) <= 0) deltaMove = Vector2.zero;
            }
            else {
                rb.position += deltaMove;
                break;
            }
        }
    }

    private void ResolveOverlap()
    {
        Collider2D[] results = new Collider2D[5];
        int count = col.Overlap(_contactFilter, results);
        
        for (int i = 0; i < count; i++) {
            ColliderDistance2D dist = col.Distance(results[i]);
            if (dist.isOverlapped) {
                rb.position += dist.normal * dist.distance;
            }
        }
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