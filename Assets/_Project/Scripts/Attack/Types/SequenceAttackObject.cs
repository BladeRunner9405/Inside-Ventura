using System.Collections;
using UnityEngine;

public class SequenceAttackObject : AttackObject
{
    [Header("Sequence Settings")]
    [Tooltip("Массив коллайдеров в том порядке, в котором они должны появляться")]
    [SerializeField] private Collider2D[] sequenceHitboxes; 
    
    [Tooltip("Общее время, за которое волна дойдет до конца")]
    [SerializeField] private float waveDuration = 0.5f;
    
    [Tooltip("Если true - старые шипы остаются на экране. Если false - исчезают (бегущая волна).")]
    [SerializeField] private bool leavePreviousActive = false;

    private Coroutine _sequenceCoroutine;
    private ContactFilter2D _contactFilter;
    private readonly Collider2D[] _overlapResults = new Collider2D[16];

    public override void Initialize(int damage, LayerMask layer, float timeToLive)
    {
        base.Initialize(damage, layer, timeToLive);

        _contactFilter.useTriggers = true;
        _contactFilter.SetLayerMask(targetLayer);
        _contactFilter.useLayerMask = true;

        foreach (var col in sequenceHitboxes)
        {
            col.gameObject.SetActive(false); 
        }

        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
        }
        _sequenceCoroutine = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (sequenceHitboxes.Length == 0) yield break;

        if (waveDuration <= 0) waveDuration = 0.5f; 

        float stepDuration = waveDuration / sequenceHitboxes.Length;

        for (int i = 0; i < sequenceHitboxes.Length; i++)
        {
            sequenceHitboxes[i].gameObject.SetActive(true);

            if (!leavePreviousActive && i > 0)
            {
                sequenceHitboxes[i - 1].gameObject.SetActive(false);
            }

            float timer = 0;
            while (timer < stepDuration)
            {
                timer += Time.deltaTime;
                CheckHits(sequenceHitboxes[i]);
                yield return null; 
            }
        }

        Despawn();
    }

    private void CheckHits(Collider2D col)
    {
        int count = Physics2D.OverlapCollider(col, _contactFilter, _overlapResults);
        
        for (int i = 0; i < count; i++)
        {
            TryDealDamage(_overlapResults[i]); 
        }
    }

    private void OnDisable()
    {
        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
        }
    }
}