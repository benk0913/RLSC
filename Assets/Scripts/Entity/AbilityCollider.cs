using EdgeworldBase;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityCollider : HitCollider
{
    static Dictionary<string, int> AbilitiesCollidersCounts = new Dictionary<string, int>();
    public bool StickToActor;
    public bool StickToActorFacing;
    public bool StickToSkilledShot;
    public bool HitEventOnWalls;

    public Vector3 InitPosition;

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

    [SerializeField]
    bool RepositionChildrenOnEnable;

    public bool OnlyForPlayers = false;

    [SerializeField]
    bool DuplicateBubbles = false; //TODO Replace with something more generic?
    public bool IsBubble = false; //TODO Replace with something more generic?
    public List<AbilityCollider> BubblesDuplicated = new List<AbilityCollider>();

    public string AudioOnHit;

    // Interval that colliders of the same type should hit the same actor. 
    public int HitInterval = 0;

    public bool RemoveOnInterrupt;

    public List<AbilityCollider> ChildrenColliders = new List<AbilityCollider>();


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
                    obj.transform.localScale = transform.localScale;
                }

                AbilityCollider abilityCol = obj.GetComponent<AbilityCollider>();
                if (abilityCol != null)
                {
                    abilityCol.SetInfo(this.AbilitySource, this.ActorSource);
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

    private void OnEnable()
    {
        InitPosition = transform.position;

        if (RepositionChildrenOnEnable)
        {
            foreach(AbilityCollider child in ChildrenColliders)
            {
                if (child.InitPosition != Vector3.zero)
                    child.transform.position = child.InitPosition;
            }
        }
    }

    private void Update()
    {
        if(StickToActor)
        {
            if(ActorSource == null)
            {
                this.gameObject.SetActive(false);
            }

            transform.position = ActorSource.transform.position;
            if (StickToActorFacing)
            {
                transform.localScale = ActorSource.Body.localScale;
            }

            if (StickToSkilledShot)
            {
                RaycastHit2D rhit = Physics2D.Raycast(SkilledShotPoint.position, Vector2.down, Mathf.Infinity, SkilledshotLayermask);
                if (rhit && Vector2.Distance(rhit.point, SkilledShotPoint.position) > 0.1f)
                {
                    CORE.Instance.LogMessageError(Vector2.Distance(rhit.point, SkilledShotPoint.position).ToString());
                    //TODO Remove?
                }
                else
                {


                    RaycastHit2D subrhit = Physics2D.Raycast(transform.position, transform.localScale.x > 0f ? Vector3.left : Vector3.right, Mathf.Infinity, SkilledshotLayermask);
                    
                    if (!subrhit)
                    {
                        return;
                    }
                    
                    rhit = Physics2D.Raycast(subrhit.point, Vector2.down, Mathf.Infinity, SkilledshotLayermask);

                    if (!rhit)
                    {
                        return;
                    }
                    
                    if (Vector2.Distance(rhit.point, SkilledShotPoint.position) < 0.1f)
                    {
                        return;
                    }
                    

                }


                transform.position = rhit.point;
            }
        }
    }

    public override void SetInfo(Ability abilitySource, Actor actorSource, string abilityInstanceId = "", HitCollider parentCollider  = null)
    {
        base.SetInfo(abilitySource, actorSource, abilityInstanceId, parentCollider);
        
        if(OnlyForPlayers && !actorSource.State.Data.isCharacter)
        {
            this.gameObject.SetActive(false);
            return;
        }

        if (AbilitySource != null)
        {
            incrementAbilityCollidersCount(AbilitySource.name);
        }

        if(StickToSkilledShot)
        {
            RaycastHit2D rhit = Physics2D.Raycast(SkilledShotPoint.position, Vector2.down, Mathf.Infinity, SkilledshotLayermask);
            if (rhit && Vector2.Distance(rhit.point, SkilledShotPoint.position) > 0.1f)
            {
                //TODO Remove?
            }
            else
            {


                RaycastHit2D subrhit = Physics2D.Raycast(transform.position, transform.localScale.x > 0f ? Vector3.left : Vector3.right, Mathf.Infinity, SkilledshotLayermask);
                
                if (!subrhit)
                {
                    return;
                }

                rhit = Physics2D.Raycast(subrhit.point, Vector2.down, Mathf.Infinity, SkilledshotLayermask);

                if (!rhit)
                {
                    return;
                }
                
                if (Vector2.Distance(rhit.point, SkilledShotPoint.position) < 0.1f)
                {
                    return;
                }
                

            }


            transform.position = rhit.point;
        }

        if(InitForce != default)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f * transform.localScale.x * InitForce.x,InitForce.y));
        }

        if(DuplicateBubbles) //TODO Replace with something more generic
        {
            BubblesDuplicated.Clear();
        }

        foreach(AbilityCollider child in ChildrenColliders)
        {
            child.SetInfo(abilitySource, actorSource, abilityInstanceId, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Actor")
        {

            Actor actorVictim = other.GetComponent<Actor>();

            AttemptHitAbility(actorVictim);
        }
        else if(other.tag == "Untagged")
        {
            if(HitEventOnWalls)
            {
                if (other.GetComponent<PlatformEffector2D>() == null)
                {
                    OnHitEvent?.Invoke();
                }
            }
        }
        else if (other.tag == "HitCollider" && other != this) //TODO Replace with something more generic?
        {

            if (DuplicateBubbles)
            {
                AbilityCollider otherAbilityCollider = other.GetComponent<AbilityCollider>();

                if (otherAbilityCollider == null || !otherAbilityCollider.IsBubble)
                {
                    return;
                }

                if(BubblesDuplicated.Contains(otherAbilityCollider) || getAbilityCollidersCount(otherAbilityCollider.AbilitySource.name) > 32)
                {
                    return;
                }

                GameObject clone = ResourcesLoader.Instance.GetRecycledObject(other.name);

                AbilityCollider cloneAbilityCollider = clone.GetComponent<AbilityCollider>();
                cloneAbilityCollider.SetInfo(otherAbilityCollider.AbilitySource, otherAbilityCollider.ActorSource, otherAbilityCollider.AbilityInstanceId);
                clone.transform.position = other.transform.position;
                clone.transform.position += clone.transform.TransformDirection(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                clone.transform.localScale = other.transform.localScale;

                BubblesDuplicated.Add(otherAbilityCollider);
                BubblesDuplicated.Add(cloneAbilityCollider);
            }
        }
    }

    protected override bool CanHitActor(Actor actorVictim)
    {
        bool CanHit = base.CanHitActor(actorVictim);
        if (CanHit && HitInterval > 0)
        {
            if (actorVictim.IsColliderOnCooldown(name))
            {
                CanHit = false;
            }
            else
            {
                actorVictim.SetColliderOnCooldown(name, HitInterval);
            }
        }
        return CanHit;
    }

    public void ForceOnHit()
    {
        OnHitEvent?.Invoke();
    }

    private void OnDisable()
    {
        if (this.CanMiss && this.AbilitySource.OnMissParams.Count > 0 && this.TimesHit == 0)
        {
            this.AttemptMissAbility();
        }
        
        if (AbilitySource != null)
        {
            decrementAbilityCollidersCount(AbilitySource.name);
        }
    }

    private void incrementAbilityCollidersCount(string abilityName)
    {
        if (AbilitiesCollidersCounts.ContainsKey(abilityName))
        {
            AbilitiesCollidersCounts[abilityName] = AbilitiesCollidersCounts[abilityName] + 1;
        }
        else 
        {
            AbilitiesCollidersCounts.Add(abilityName, 1);
        }
    }

    private void decrementAbilityCollidersCount(string abilityName)
    {
        if (AbilitiesCollidersCounts.ContainsKey(abilityName))
        {
            AbilitiesCollidersCounts[abilityName] = AbilitiesCollidersCounts[abilityName] - 1;

            if (AbilitiesCollidersCounts[abilityName] == 0)
            {
                AbilitiesCollidersCounts.Remove(abilityName);
            }
        }
    }

    private int getAbilityCollidersCount(string abilityName)
    {
        if (AbilitiesCollidersCounts.ContainsKey(abilityName))
        {
            return AbilitiesCollidersCounts[abilityName];
        }
        return 0;
    }
}
