using UnityEngine;

public class CircleAttackObject : AttackObject
{
    [Header("Circle Settings")]
    [SerializeField] private float radius = 1f;

    // Переопределяем только геометрию удара!
    protected override void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            TryDealDamage(hit);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}