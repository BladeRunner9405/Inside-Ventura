using UnityEngine;

public class ArmRotation : MonoBehaviour
{
  [SerializeField] private GameObject hand;
  [SerializeField] private float radius = 1f;
  [SerializeField] private float angleOffset = 0f;
  [SerializeField] private bool flipOnLeftSide = true;

  private Transform handTransform;
  private SpriteRenderer handSprite;

  private void Awake()
  {
    handTransform = hand.transform;
    handSprite = hand.GetComponent<SpriteRenderer>();
  }

  public void rotateAt(Vector3 targetWorldPosition)
  {
    SetPositionAtDistance(targetWorldPosition);

    Vector2 direction;

    float distToArm = Vector3.Distance(transform.position, targetWorldPosition);

    if (distToArm < radius)
    {
      Vector3 outward = (handTransform.position - transform.position).normalized;
      direction = outward;
    }
    else
    {
      direction = (targetWorldPosition - handTransform.position).normalized;
    }

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    angle += angleOffset;

    handTransform.rotation = Quaternion.Euler(0f, 0f, angle);

    if (flipOnLeftSide)
    {
      float isLeftMultiplier = Mathf.Abs(angle) < 90f || Mathf.Abs(angle) > 270f ? -1f : 1f;
      Vector3 currentScale = handTransform.localScale;
      handTransform.localScale = new Vector3(currentScale.x, Mathf.Abs(currentScale.y) * isLeftMultiplier, currentScale.z);
    }
  }

  private void SetPositionAtDistance(Vector3 targetWorldPosition)
  {
    Vector3 directionToTarget = (targetWorldPosition - transform.position).normalized;
    Vector3 desiredLocalPosition = directionToTarget * radius;

    handTransform.localPosition = desiredLocalPosition;
  }
}
