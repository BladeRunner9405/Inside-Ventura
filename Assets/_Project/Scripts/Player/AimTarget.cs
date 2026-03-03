using UnityEngine;

public class AimTarget: MonoBehaviour
{
    [SerializeField] private float aimDeltaModifier;
    [SerializeField] private float maxAimDist;

    public void aimAt(Vector3 coords) // прицелиться на объект с координатами coords
    {
        Vector3 localPosition = Vector3.ClampMagnitude((coords - transform.position) * aimDeltaModifier, maxAimDist);
        transform.localPosition = localPosition;
    }
}
