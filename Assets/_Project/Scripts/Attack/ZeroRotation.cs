using UnityEngine;

public class ZeroRotation : MonoBehaviour
{
    void LateUpdate()
    {
        if (transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}