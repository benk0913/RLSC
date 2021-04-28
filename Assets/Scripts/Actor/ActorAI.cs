using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAI : MonoBehaviour
{
    #region Essentials 

    [SerializeField]
    public Actor Act;

    [SerializeField]
    public AIChaseBehaviour ChaseBehaviour;

    [SerializeField]
    public bool IsBoss;

    [SerializeField]
    protected LayerMask groundLayerMask;

    [SerializeField]
    protected float NoticeDistance = 0f;

    public bool Asleep = true;

    [SerializeField]
    protected float ChaseDistance = 1f;

    [SerializeField]
    protected float FleeDistance = 10f;

    public bool HasTargetingLine;

    public bool IsJumping = false;

    public Actor CurrentTarget
    {
        get
        {
            return GetCurrentTarget();
        }
    }


    protected bool lastIsBitch;

    protected RaycastHit2D rhitLeft;
    protected RaycastHit2D rhitRight;

    protected bool patrolDirection;

    protected TargetingEntity targetingEntity;

    protected void OnEnable()
    {
        if(ChaseBehaviour == AIChaseBehaviour.Static || !HasTargetingLine)
        {
            return;
        }

        targetingEntity = ResourcesLoader.Instance.GetRecycledObject("MonsterTargetingEffect").GetComponent<TargetingEntity>();
        targetingEntity.SetInfo(this);
        
    }

    protected virtual void Awake()
    {
        CORE.Instance.SubscribeToEvent("BitchChanged", OnBitchChanged);
        
        OnBitchChanged();

    }

    protected virtual void Start()
    {
        if(ChaseBehaviour == AIChaseBehaviour.Patrol || ChaseBehaviour == AIChaseBehaviour.Chase)
        {
            System.Action patrolDirectionChangeAction = null;

            patrolDirectionChangeAction = () => 
            {
                if (this == null || !this.gameObject.activeInHierarchy)
                {
                    return;
                }

                patrolDirection = (Random.Range(0, 2) == 0);

                CORE.Instance.DelayedInvokation(3f, patrolDirectionChangeAction);
                
            };

            CORE.Instance.DelayedInvokation(3f, patrolDirectionChangeAction);
        }

        
        
    }

    public virtual Actor GetCurrentTarget()
    {

        if (ChaseBehaviour == AIChaseBehaviour.Chase && NoticeDistance > 0f && Asleep) //TODO Should probably replace with a less performance heavy implementation
        {
            if(CORE.Instance.Room.Actors.Find(X =>
                !X.isMob
                && X.ActorEntity != null 
                && Vector2.Distance(X.ActorEntity.transform.position, transform.transform.position) < NoticeDistance
                && !X.ActorEntity.IsDead) != null)
            {
                Asleep = false;
                return null;
            }

            return null;
        }

        return CORE.Instance.Room.MostThreateningActor;
    }


    protected virtual void OnBitchChanged()
    {
        if (!CORE.Instance.IsBitch)
        {
            StopAllCoroutines();
        }
        else
        {
            if (lastIsBitch != CORE.Instance.IsBitch)
            {
                StartCoroutine(AIBehaviourRoutine());
            }
        }

        lastIsBitch = CORE.Instance.IsBitch;

        Act.RefreshControlSource();
    }

    protected virtual void Update()
    {
        RefreshRaycasts();
    }

    protected void RefreshRaycasts()
    {
        rhitLeft  = Physics2D.Raycast(transform.position + new Vector3(0f, 1f, 0f), Vector2.left, Act.GroundCheckDistance, groundLayerMask);
        rhitRight = Physics2D.Raycast(transform.position + new Vector3(0f, 1f, 0f), Vector2.right, Act.GroundCheckDistance, groundLayerMask);
    }

    #endregion

    #region Layer1

    protected virtual AbilityState GetAvailableAbilityState()
    {
        List<AbilityState> abilities = new List<AbilityState>();

        abilities.AddRange(Act.State.Abilities.FindAll(x => x.CurrentCD <= 0));

        if(abilities.Count == 0)
        {
            return null;
        }

        return abilities[Random.Range(0, abilities.Count)];
    }
    
    protected virtual void MoveToTarget()
    {
        if (CurrentTarget == null)
        {
            return;
        }

        if(ChaseBehaviour == AIChaseBehaviour.Static)
        {
            return;
        }

        if(Mathf.Abs(CurrentTarget.transform.position.x - transform.position.x) < Act.GroundedDistance)
        {
            return;
        }

        if (IsJumping)
        {
            if (CurrentTarget.transform.position.x > transform.position.x)
            {
                if (rhitRight)
                {
                    Act.AttemptJump();
                }

                Act.AttemptMoveRight();
                return;
            }
            else if (CurrentTarget.transform.position.x < transform.position.x)
            {
                if (rhitLeft)
                {
                    Act.AttemptJump();
                }

                Act.AttemptMoveLeft();
                return;
            }
        }
        else
        {
            if (CurrentTarget.transform.position.x > transform.position.x && !rhitRight)
            {
                Act.AttemptMoveRight();
                return;
            }
            else if (CurrentTarget.transform.position.x < transform.position.x && !rhitLeft)
            {
                Act.AttemptMoveLeft();
                return;
            }
        }

        if (rhitLeft && rhitRight)
        {
            Act.AttemptMoveLeft();
        }

    }

    protected void MoveFromTarget()
    {
        if(CurrentTarget == null)
        {
            return;
        }

        if (CurrentTarget.transform.position.x > transform.position.x && !rhitLeft)
        {
            Act.AttemptMoveLeft();
        }
        else if (CurrentTarget.transform.position.x < transform.position.x && !rhitRight)
        {
            Act.AttemptMoveRight();
        }
    }

    #endregion

    #region Layer2
    
    protected virtual void WaitBehaviour()
    {
        switch (ChaseBehaviour)
        {
            case AIChaseBehaviour.Static:
                {
                    

                    break;
                }
            case AIChaseBehaviour.Chase:
                {
                    
                    if (CurrentTarget == null)
                    {
                        break;
                    }

                    if (Vector2.Distance(transform.position, CurrentTarget.transform.position) < ChaseDistance)
                    {

                        break;
                    }

                    MoveToTarget();
                    break;
                }
            case AIChaseBehaviour.Patrol:
                {
                    if(patrolDirection)
                    {
                        Act.AttemptMoveLeft();
                    }
                    else
                    {
                        Act.AttemptMoveRight();
                    }

                    break;
                }
            case AIChaseBehaviour.Flee:
                {
                    if (Vector2.Distance(transform.position, CurrentTarget.transform.position) > FleeDistance)
                    {
                        break;
                    }

                    MoveFromTarget();

                    break;
                }
        }
    }

    protected virtual IEnumerator UseAbilityRoutine(AbilityState selectedAbility)
    {

        //It is recommended to change this behaviour with each AI, so monsters will use their abilities wisely.
        MoveToTarget();

        yield return 0;

        Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));

        yield return new WaitForSeconds(selectedAbility.CurrentAbility.CastingTime);
    }

    protected IEnumerator AreaPatrolRoutine()
    {

        float t = Random.Range(0.5f, 1.5f);
        while (t > 0f)
        {
            t -= Time.deltaTime;

            if (patrolDirection)
            {
                if (!rhitLeft)
                {
                    Act.AttemptMoveLeft();
                }
            }
            else
            {
                if (!rhitRight)
                {
                    Act.AttemptMoveRight();
                }
            }

            yield return 0;
        }

        if (CurrentTarget == null)
        {
            yield break;
        }

        while (Vector2.Distance(transform.position, CurrentTarget.transform.position) > ChaseDistance)
        {
            MoveToTarget();
            yield return 0;
        }

    }
    #endregion

    #region Highest Layer

    protected virtual IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = GetAvailableAbilityState();
                WaitBehaviour();

                yield return 0;
            }
            
            yield return new WaitForSeconds(Random.Range(0f,0.2f));

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
            

            yield return 0;
        }
    }

    
    #endregion
}

[System.Serializable]
public enum AIChaseBehaviour
{
    Static,
    Patrol,
    Chase,
    Flee
}