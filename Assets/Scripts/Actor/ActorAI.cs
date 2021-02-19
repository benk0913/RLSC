using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAI : MonoBehaviour
{
    #region Essentials 

    [SerializeField]
    public Actor Act;

    [SerializeField]
    public Monster MonsterRef;

    [SerializeField]
    protected LayerMask groundLayerMask;

    [SerializeField]
    protected float ChaseDistance = 1f;

    [SerializeField]
    protected float FleeDistance = 10f;

    public Actor CurrentTarget
    {
        get
        {
            if(_currentTarget == null)
            {
                _currentTarget =  GetCurrentTarget();
            }

            return _currentTarget;
        }
        set
        {
             _currentTarget = value;
        }
    }
    Actor _currentTarget;


    protected bool lastIsBitch;

    protected RaycastHit2D rhitLeft;
    protected RaycastHit2D rhitRight;

    protected bool patrolDirection;

    protected virtual void Awake()
    {
        CORE.Instance.SubscribeToEvent("BitchChanged", OnBitchChanged);
        
        OnBitchChanged();

    }

    protected virtual void Start()
    {
        if(MonsterRef.ChaseBehaviour == AIChaseBehaviour.Patrol || MonsterRef.ChaseBehaviour == AIChaseBehaviour.Chase)
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
        return CORE.Instance.Room.MostThreateningActor;
    }


    protected virtual void OnBitchChanged()
    {
        if(MonsterRef == null)
        {
            return;
        }

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

        if (CurrentTarget.transform.position.x > transform.position.x && !rhitRight)
        {
            Act.AttemptMoveRight();
        }
        else if (CurrentTarget.transform.position.x < transform.position.x && !rhitLeft)
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
        switch (MonsterRef.ChaseBehaviour)
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
