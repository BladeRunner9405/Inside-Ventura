using System.Collections;
using UnityEngine;

namespace InsideVentura.World
{
  public class MoneyItem : Item
  {
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float arriveDistance = 0.1f;

    private Coroutine _moveCoroutine;

    protected override void OnEnable()
    {
      base.OnEnable();
      OnPlayerNearby += OnPickup;
    }

    private void OnDisable()
    {
      OnPlayerNearby -= OnPickup;
      if (_moveCoroutine != null)
        StopCoroutine(_moveCoroutine);
    }

    protected override void OnPickup()
    {
      base.OnPickup();
      if (_moveCoroutine != null) return;
      _moveCoroutine = StartCoroutine(MoveToPlayerAndDeactivate());
    }

    private IEnumerator MoveToPlayerAndDeactivate()
    {
      var playerTransform = PlayerAccessor.Transform;

      while (true)
      {
        var targetPos = playerTransform.position;
        var newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        transform.position = newPos;

        if (Vector3.Distance(transform.position, targetPos) <= arriveDistance)
        {
          transform.position = targetPos;
          break;
        }

        yield return null;
      }

      Collect();
    }

    private void Collect()
    {
      PlayerAccessor.Stats.Money.Change(StatOperationType.Add, 1);
      Deactivate();
    }
  }
}
