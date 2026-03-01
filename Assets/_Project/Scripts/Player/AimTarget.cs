using UnityEngine;

public class AimTarget: MonoBehaviour
{
    public float aimDeltaModifier;
    public float maxAimDist;

    public void aimAt(Vector3 coords) // прицелиться на объект с координатами coords
    {
        Vector3 localPosition = Vector3.ClampMagnitude((coords - transform.position) * aimDeltaModifier, maxAimDist); 
        transform.localPosition = new Vector3(localPosition.x, 
                                           localPosition.y,  
                                           localPosition.z);
    }
}
