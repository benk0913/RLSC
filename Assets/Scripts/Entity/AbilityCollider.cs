﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCollider : MonoBehaviour
{
    public Ability AbilitySource;
    public Actor ActorSource;

    public UnityEvent OnHitEvent;

    public void SetInfo(Ability abilitySource, Actor actorSource)
    {
        AbilitySource = abilitySource;
        ActorSource = actorSource;
    }

    private void OnTriggerEnter(Collider other)
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

    public void HitActor(Actor actor)
    {
        OnHitEvent?.Invoke();

        JSONNode node = new JSONClass();
        node["hitData"]["actorId"] = actor.State.Data.actorId;
        node["hitData"]["abilityName"] = AbilitySource.name;

        SocketHandler.Instance.SendEvent("ability_hit",node);
    }
}
