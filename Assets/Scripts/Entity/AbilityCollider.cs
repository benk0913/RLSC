using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCollider : MonoBehaviour
{
    public Ability AbilitySource;
    public Actor ActorSource;

    public UnityEvent OnHitEvent;

    public bool StickToActor;
    public bool StickToSkilledShot;
    public bool HitEventOnWalls;

    [SerializeField]
    Transform SkilledShotPoint;

    [SerializeField]
    LayerMask SkilledshotLayermask;

    [SerializeField]
    Vector3 InitForce;

    [SerializeField]
    string onHitObject;

    [SerializeField]
    Transform OnHitSource;


    public string AudioOnHit;

    public TargetType targetType = TargetType.Enemies;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(onHitObject))
        {
            OnHitEvent.AddListener(() =>
            {
                GameObject obj = ResourcesLoader.Instance.GetRecycledObject(onHitObject);
                if (OnHitSource != null)
                {
                    obj.transform.position = OnHitSource.position;
                }
                else
                {
                    obj.transform.position = transform.position;
                }
            });
        }

        if (!string.IsNullOrEmpty(AudioOnHit))
        {
            OnHitEvent.AddListener(() =>
            {

                if (OnHitSource != null)
                {
                    AudioControl.Instance.PlayInPosition(AudioOnHit, OnHitSource.position);
                }
                else
                {
                    AudioControl.Instance.PlayInPosition(AudioOnHit, transform.position);
                }
            });
        }
    }

    private void Update()
    {
        if(StickToActor)
        {
            transform.position = ActorSource.transform.position;
        }
    }

    public void SetInfo(Ability abilitySource, Actor actorSource)
    {
        AbilitySource = abilitySource;
        ActorSource = actorSource;

        if(StickToSkilledShot)
        {
            RaycastHit2D rhit = Physics2D.Raycast(SkilledShotPoint.position, Vector2.down, Mathf.Infinity, SkilledshotLayermask);
            if(rhit)
            {
                transform.position = rhit.point;
            }
        }

        if(InitForce != default)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f * transform.localScale.x * InitForce.x,InitForce.y));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {   

            Actor actorVictim = other.GetComponent<Actor>();


            
            if(targetType == TargetType.Self)
            {
                if(actorVictim != ActorSource)
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

            if (actorVictim.State.Data.actorId != CORE.Instance.Room.PlayerActor.actorId) //is not the players actor
            {
                
                return;
            }

            JSONNode node = new JSONClass();
            node["casterActorId"] = ActorSource.State.Data.actorId;
            node["targetActorId"] = actorVictim.State.Data.actorId;
            node["abilityName"] = AbilitySource.name;

            SocketHandler.Instance.SendEvent("ability_hit", node);
        }
        else if(other.tag == "Untagged")
        {
            if(HitEventOnWalls)
            {
                OnHitEvent?.Invoke();
            }
        }
    }


    public void ForceOnHit()
    {
        OnHitEvent?.Invoke();
    }

}
