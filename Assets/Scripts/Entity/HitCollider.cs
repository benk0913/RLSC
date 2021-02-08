﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour
{
    [SerializeField]
    public Actor ActorSource;

    [SerializeField]
    public Ability AbilitySource;

    [SerializeField]
    public TargetType targetType;

    [SerializeField]
    public UnityEvent OnHitEvent;

    [SerializeField]
    public bool CanMiss;

    [SerializeField]
    public bool HitOnlyGroundedActors = false;

    protected int TimesHit = 0;


    public virtual void SetInfo(Ability abilitySource = null, Actor actorSource = null)
    {
        AbilitySource = abilitySource;
        ActorSource = actorSource;
        TimesHit = 0;
    }

    public virtual void AttemptHitAbility(Actor actorVictim)
    {
        if (!CanHitActor(actorVictim))
        {
            return;
        }
        
        if (AbilitySource.TargetCap > 0 && TimesHit >= AbilitySource.TargetCap)
        {
            return;
        }

        TimesHit++;
        OnHitEvent?.Invoke();

        if (!CanSendEventForActor(actorVictim))
        {
            return;
        }

        JSONNode node = new JSONClass();
        node["casterActorId"] = ActorSource.State.Data.actorId;
        node["targetActorId"] = actorVictim.State.Data.actorId;
        node["abilityName"] = AbilitySource.name;

        SocketHandler.Instance.SendEvent("ability_hit", node);
    }

    public virtual void AttemptMissAbility()
    {
        if (!CanSendEventForActor(ActorSource))
        {
            return;
        }

        JSONNode node = new JSONClass();
        node["actorId"] = ActorSource.State.Data.actorId;
        node["abilityName"] = AbilitySource.name;

        SocketHandler.Instance.SendEvent("ability_miss", node);
    }

    private bool CanHitActor(Actor actorVictim)
    {
        bool isVictimAlly = ActorSource.State.Data.isMob == actorVictim.State.Data.isMob;

        if (targetType == TargetType.Self)
        {
            if (actorVictim != ActorSource)
            {
                return false;
            }
        }
        else if (targetType == TargetType.Enemies)
        {
            if (actorVictim == ActorSource)
            {
                return false;
            }

            if (isVictimAlly)
            {
                return false;
            }
        }
        else if (targetType == TargetType.Friends)
        {
            if (actorVictim == ActorSource)
            {
                return false;
            }

            if (!isVictimAlly)
            {
                return false;
            }
        }
        else if (targetType == TargetType.FriendsAndSelf)
        {
            if (!isVictimAlly)
            {
                return false;
            }
        }
        else if (targetType == TargetType.NotSelf)
        {
            if (actorVictim == ActorSource)
            {
                return false;
            }
        }

        if (HitOnlyGroundedActors && !actorVictim.IsGrounded)
        {
            return false;
        }

        if (!isVictimAlly && actorVictim.IsInvulnerable)
        {
            return false;
        }

        return true;
    }

    private bool CanSendEventForActor(Actor targetVictim)
    {
        if (CORE.Instance.IsBitch)
        {
            if (targetVictim.State.Data.isCharacter && targetVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId)
            {
                return false;
            }
        }
        else
        {
            if (targetVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId) // is not the players actor
            {
                return false;
            }
        }
        return true;
    }
}
