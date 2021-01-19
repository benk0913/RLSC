using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuffCollider : HitCollider
{
    public Buff BuffSource;

    public bool StickToActor;

    public bool HitCollider;


    protected void Update()
    {
        if(StickToActor)
        {
            if(ActorSource == null)
            {
                this.gameObject.SetActive(false);
                return;
            }
            transform.position = ActorSource.transform.position;
        }
    }

    public void SetInfo(Buff buffSource, Actor actorSource)
    {
        BuffSource = buffSource;
        ActorSource = actorSource;

        SetInfo(buffSource.HitAbility, actorSource);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {
            Actor actorVictim = other.GetComponent<Actor>();
            if (HitCollider)
            {
                AttemptHitAbility(actorVictim);
            }


        }

    }

    
}
