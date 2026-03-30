using System.Collections;
using UnityEngine;

public class SequenceAttackObject : AttackObject {
  [Header("Sequence Settings")]
  [Tooltip("Массив коллайдеров в том порядке, в котором они должны появляться")]
  [SerializeField]
  private Collider2D[] sequenceHitboxes;

  [Tooltip("Общее время, за которое волна дойдет до конца")] [SerializeField]
  private float waveDuration = 0.5f;

  [Tooltip("Если true - старые шипы остаются на экране. Если false - исчезают (бегущая волна).")] [SerializeField]
  private bool leavePreviousActive;

  private readonly Collider2D[] _overlapResults = new Collider2D[16];
  private ContactFilter2D _contactFilter;

  private Coroutine _sequenceCoroutine;

  private void OnDisable() {
    if (_sequenceCoroutine != null) {
      StopCoroutine(_sequenceCoroutine);
      _sequenceCoroutine = null;
    }
  }

  public override void Initialize(float damage, LayerMask layer, float timeToLive) {
    base.Initialize(damage, layer, timeToLive);

    _contactFilter.useTriggers = true;
    _contactFilter.SetLayerMask(targetLayer);
    _contactFilter.useLayerMask = true;

    foreach (var col in sequenceHitboxes) col.gameObject.SetActive(false);

    if (_sequenceCoroutine != null) StopCoroutine(_sequenceCoroutine);
    _sequenceCoroutine = StartCoroutine(PlaySequence());
  }

  private IEnumerator PlaySequence() {
    if (sequenceHitboxes.Length == 0) yield break;

    if (waveDuration <= 0) waveDuration = 0.5f;

    var stepDuration = waveDuration / sequenceHitboxes.Length;

    for (var i = 0; i < sequenceHitboxes.Length; i++) {
      sequenceHitboxes[i].gameObject.SetActive(true);

      if (!leavePreviousActive && i > 0) sequenceHitboxes[i - 1].gameObject.SetActive(false);

      float timer = 0;
      while (timer < stepDuration) {
        timer += Time.deltaTime;
        CheckHits(sequenceHitboxes[i]);
        yield return null;
      }
    }

    Despawn();
  }

  private void CheckHits(Collider2D col) {
    var count = Physics2D.OverlapCollider(col, _contactFilter, _overlapResults);

    for (var i = 0; i < count; i++) TryDealDamage(_overlapResults[i]);
  }
}
