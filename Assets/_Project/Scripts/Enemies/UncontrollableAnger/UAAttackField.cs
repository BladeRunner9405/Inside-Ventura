using System;
using System.Collections.Generic;
using CherryFramework.DependencyManager;
using DG.Tweening;
using UnityEngine;

public class UAAttackField : InjectMonoBehaviour {
  [HideInInspector] public int damage;

  [Inject] protected PlayerAccessor PlayerAccessor;

  [SerializeField] private List<BoxCollider2D> hitboxes = new();
  [SerializeField] private float attackWaveDuration = 0.5F;

  private int _curHitbox = -1;
  private int _lastHitbox = -1;

  private void Start() {
    _curHitbox = 0;
    Debug.Log(hitboxes.Count);
    DOTween.Sequence()
      .Append(DOTween.To(() => _curHitbox, UpdateHitbox, 3, attackWaveDuration))
      .AppendCallback(() => {
        _curHitbox = -1;
        // Debug.Log("Wave sequence ended");
        Destroy(gameObject);
      }).PlayForward();
  }

  private void UpdateHitbox(int newVal) {
    if (newVal == _lastHitbox) {
      return;
    }

    // Debug.Log($"Updating hitbox with new index {newVal}");
    _curHitbox = newVal;
    _lastHitbox = newVal;
    if (hitboxes[_curHitbox].OverlapPoint(PlayerAccessor.Player.transform.position)) {
      PlayerAccessor.Player.TakeDamage(damage);
    }
  }

  private void OnDrawGizmos() {
    if (_curHitbox != -1) {
      var curCollider = hitboxes[_curHitbox];
      Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(curCollider.transform.localPosition, new Vector3(1, 1, 0));
    }
  }
}
