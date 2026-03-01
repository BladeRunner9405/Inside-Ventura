using UnityEngine;

public class Entity: MonoBehaviour
{
    public float MoveSpeed;
    public Transform target; // Transform, в чью сторону смотрит Entity

    public void moveTo(Vector2 dir) // переместиться в направлении dir
    {
        dir *= Time.deltaTime * MoveSpeed;
        transform.localPosition = new Vector3(transform.position.x + dir.x, 
                                         transform.position.y + dir.y,  
                                         transform.position.z);
    }

    public void lookAt(Vector3 coords) // посмотреть на объект с координатами coords
    {
        // ...
    }

}
