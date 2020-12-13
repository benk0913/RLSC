using SimpleJSON;
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


    public virtual void SetInfo(Ability abilitySource, Actor actorSource)
    {
        AbilitySource = abilitySource;
        ActorSource = actorSource;
    }

    public virtual void AttemptHitAbility(Actor actorVictim)
    {

        if (actorVictim.IsInvulnerable)
        {
            return;
        }

        if (targetType == TargetType.Self)
        {
            if (actorVictim != ActorSource)
            {
                return;
            }
        }
        else if (targetType == TargetType.Enemies)
        {
            if (actorVictim == ActorSource)
            {
                return;
            }

            if (ActorSource.State.Data.isMob && actorVictim.State.Data.isMob)
            {
                return;
            }

            if (ActorSource.State.Data.isCharacter && actorVictim.State.Data.isCharacter)
            {
                return;
            }
        }
        else if (targetType == TargetType.Friends)
        {
            if (actorVictim == ActorSource)
            {
                return;
            }

            if (ActorSource.State.Data.isMob && actorVictim.State.Data.isCharacter)
            {
                return;
            }

            if (ActorSource.State.Data.isCharacter && actorVictim.State.Data.isMob)
            {
                return;
            }
        }
        else if (targetType == TargetType.NotSelf)
        {
            if (actorVictim == ActorSource)
            {
                return;
            }
        }

        OnHitEvent?.Invoke();

        if (CORE.Instance.IsBitch)
        {
            if (actorVictim.State.Data.IsPlayer && actorVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId)
            {
                return;
            }
        }
        else
        {
            if (actorVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId) //is not the players actor
            {
                return;
            }
        }

        JSONNode node = new JSONClass();
        node["casterActorId"] = ActorSource.State.Data.actorId;
        node["targetActorId"] = actorVictim.State.Data.actorId;
        node["abilityName"] = AbilitySource.name;

        SocketHandler.Instance.SendEvent("ability_hit", node);
    }
}
