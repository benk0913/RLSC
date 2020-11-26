using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChaseEntity : MonoBehaviour
{
    [SerializeField]
    Transform ReferenceObject;

    public float Speed = 1f;


    public float YOffset = 3f;

    void LateUpdate()
    {
        if (ReferenceObject == null)
        {
            ActorData foundActor = CORE.Instance.Room.Actors.Find(X => X.IsPlayer);

            if (foundActor != null)
            {
                ReferenceObject = foundActor.ActorObject.transform;
            }

            return;
        }

        transform.position = Vector3.Lerp(transform.position,
            new Vector3(ReferenceObject.transform.position.x, ReferenceObject.transform.position.y + YOffset, transform.position.z), 
            Speed * Time.deltaTime);
    }
}
