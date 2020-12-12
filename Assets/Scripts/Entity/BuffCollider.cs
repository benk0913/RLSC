using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuffCollider : MonoBehaviour
{
    public Buff BuffSource;
    public Actor ActorSource;

    public UnityEvent OnHitEvent;

    public bool StickToActor;


    private void Update()
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {   
            Actor actorVictim = other.GetComponent<Actor>();

            if(actorVictim == ActorSource) // is the initiator
            {
                return;
            }

            if (actorVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId) //is not the players actor
            {
                return;
            }

            HitActor(actorVictim);
        }
    }

    public void HitActor(Actor targetActor)
    {
        OnHitEvent?.Invoke();
        //TODO Server will probably not expect an ability with a buff name....
        JSONNode node = new JSONClass();
        node["casterActorId"] = ActorSource.State.Data.actorId;
        node["targetActorId"] = targetActor.State.Data.actorId;
        node["abilityName"] = BuffSource.name;

        SocketHandler.Instance.SendEvent("ability_hit",node);
    }
}
