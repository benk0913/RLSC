using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChaseEntity : MonoBehaviour
{
    [SerializeField]
    Transform ReferenceObject;

    public float Speed = 1f;


    public float YOffset = 3f;

    public float Extrapolation = 1f;

    Vector3 deltaPosition;
    Vector3 lastPosition;

    void LateUpdate()
    {
        deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;

        if (ReferenceObject == null)
        {
            ActorData foundActor = CORE.Instance.Room.PlayerActor;

            if (foundActor != null && foundActor.ActorEntity != null)
            {
                ReferenceObject = foundActor.ActorEntity.transform;
            }

            return;
        }
        
        Vector3 targetPosition = new Vector3(ReferenceObject.transform.position.x + ((deltaPosition.x > 0? 1f:-1f)*Extrapolation), ReferenceObject.transform.position.y + YOffset, transform.position.z);

        
        transform.position = Vector3.Lerp(transform.position,
            targetPosition, 
            Speed * Time.deltaTime);
    }
}
