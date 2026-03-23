using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerAttackObject : AttackObject 
{
    private void Awake() 
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        TryDealDamage(other);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);
    }
}