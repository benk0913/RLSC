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
    public Actor CasterActor;

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

    public string AbilityInstanceId;

    public HitCollider ParentCollider;

    public virtual void SetInfo(Ability abilitySource = null, Actor actorSource = null, string abilityInstanceId = "", HitCollider parentCollider = null)
    {
        this.ParentCollider = parentCollider;

        AbilitySource = abilitySource;
        ActorSource = actorSource;
        AbilityInstanceId = abilityInstanceId;
        TimesHit = 0;
    }

    protected virtual bool AttemptHitAbility(Actor actorVictim)
    {
        if (!CanHitActor(actorVictim))
        {
            
            return false;
        }

        
        if(ParentCollider != null)
        {
            return ParentCollider.AttemptHitAbility(actorVictim);
        }

        if (CanSendEventForActor(actorVictim))
        {
            JSONNode node = new JSONClass();
            node["casterActorId"] = ActorSource.State.Data.actorId;
            node["targetActorId"] = actorVictim.State.Data.actorId;
            node["abilityName"] = AbilitySource.name;
            node["abilityInstanceId"] = AbilityInstanceId;

            SocketHandler.Instance.SendEvent("ability_hit", node);
        }

        TimesHit++;
        OnHitEvent?.Invoke();

        return true;
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
        node["abilityInstanceId"] = AbilityInstanceId;

        SocketHandler.Instance.SendEvent("ability_miss", node);
    }

    protected virtual bool CanHitActor(Actor actorVictim)
    {
        if(ActorSource == null || actorVictim == null)
        {
            CORE.Instance.LogMessageError("NO ACTOR? "+ActorSource+" "+actorVictim);
            return false;
        }
        //TODO - This is just a null check - didnt solve why theres no "State.Data" sometimes...
        if(ActorSource.State.Data == null ||  actorVictim.State.Data == null)
        {
            CORE.Instance.LogMessageError("NO ACTOR? "+ActorSource+" "+actorVictim);
            return false;
        }

        if (ActorSource.IsDead)
        {
            return false;
        }

        bool inSameParty = ActorSource.State.Data.actorId == actorVictim.State.Data.actorId || (ActorSource.State.Data.IsPlayer && actorVictim.State.Data.isBot);
        if (CORE.Instance != null && CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members != null && CORE.Instance.CurrentParty.members.Length > 1)
        {
            int matchingMembers = 0;

            foreach (string member in CORE.Instance.CurrentParty.members)
            {
                if (ActorSource.State.Data.name == member || actorVictim.State.Data.name == member)
                {
                    matchingMembers++;
                }
            }
            inSameParty = inSameParty || matchingMembers == 2;
        }

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

            if (!CORE.Instance.ActiveSceneInfo.enablePvp && isVictimAlly)
            {
                return false;
            }

            if (CORE.Instance.ActiveSceneInfo.enablePvp && inSameParty)
            {
                return false;
            }

            if (actorVictim.IsDead)
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

            if (!ActorSource.State.Data.isMob && !inSameParty)
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

            if (!ActorSource.State.Data.isMob && !inSameParty)
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

        if (inSameParty && actorVictim.IsFriendsInvulnerable && !actorVictim.IsDead)
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
