using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxTilingSprite : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer Renderer;

    [SerializeField]
    Transform ReferenceObject;

    public float Speed = 1f;

    Vector3 LastPos;

    void Update()
    {
        if (ReferenceObject == null)
        {
            ActorData foundActor = CORE.Instance.Room.Actors.Find(X => X.IsPlayer);

            if(foundActor != null && foundActor.ActorObject != null)
            {
                ReferenceObject = foundActor.ActorObject.transform;
            }

            return;
        }

        Renderer.material.mainTextureOffset = new Vector2(Renderer.material.mainTextureOffset.x + ((ReferenceObject.position - LastPos).x * Speed* Time.deltaTime), Renderer.material.mainTextureOffset.y);

        LastPos = ReferenceObject.position;
    }
}
