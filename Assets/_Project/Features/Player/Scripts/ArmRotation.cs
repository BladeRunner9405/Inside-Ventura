using UnityEngine;

public class ArmRotation : MonoBehaviour
{
  [SerializeField] private bool flipOnLeftSide = true;
  [SerializeField] private float angleOffset = 0f;
  [SerializeField] private SpriteRenderer armSprite;

  private Transform armTransform;

  private void Awake()
  {
    armTransform = transform;
  }

  public void RotateToTarget(Vector3 targetWorldPosition)
  {
    Vector2 direction = (targetWorldPosition - armTransform.position).normalized;

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    angle += angleOffset;

    armTransform.rotation = Quaternion.Euler(0f, 0f, angle);

    if (flipOnLeftSide && armSprite != null)
    {
      bool isLeft = Mathf.Abs(angle) > 90f;
      armSprite.flipY = isLeft;
    }
  }
}
