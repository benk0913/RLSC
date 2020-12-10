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
    LayerMask groundLayerMask;

    public Actor CurrentTarget;


    protected bool lastIsBitch;

    protected RaycastHit2D rhitLeft;
    protected RaycastHit2D rhitRight;

    protected virtual void Awake()
    {
        CORE.Instance.SubscribeToEvent("BitchChanged", OnBitchChanged);
        
        OnBitchChanged();
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
        rhitLeft  = Physics2D.Raycast(transform.position + new Vector3(0f, 1f, 0f), Vector2.left, 1f, groundLayerMask);
        rhitRight = Physics2D.Raycast(transform.position + new Vector3(0f, 1f, 0f), Vector2.left, 1f, groundLayerMask);
    }

    #endregion

    #region Layer1

    protected virtual AbilityState GetAvailableAbilityState()
    {
        List<AbilityState> abilities = new List<AbilityState>();

        abilities.AddRange(Act.State.Abilities.FindAll(x => x.CurrentCD <= 0));

        return abilities[Random.Range(0, abilities.Count)];
    }
    
    protected void MoveToTarget()
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

    protected virtual Actor GetCurrentTarget()
    {
        return CORE.Instance.Room.MostThreateningActor;
    }

    #endregion

    #region Layer2
    
    protected virtual IEnumerator WaitBehaviourRoutine()
    {
        switch(MonsterRef.ChaseBehaviour)
        {
            case AIChaseBehaviour.Static:
                {
                    yield return new WaitForSeconds(1f);
                    break;
                }
            case AIChaseBehaviour.Chase:
                {
                    float t = 0f;
                    while(t<1f)
                    {
                        t+= Time.deltaTime;

                        MoveToTarget();

                        yield return 0;
                    }
                    break;
                }
            case AIChaseBehaviour.Patrol:
                {
                    float t = 0f;

                    bool toRight = Random.Range(0, 2) == 0;

                    while (t < 1f)
                    {
                        t += Time.deltaTime;

                        if (toRight)
                        {
                            Act.AttemptMoveRight();
                        }
                        else
                        {
                            Act.AttemptMoveLeft();
                        }

                        yield return 0;
                    }
                    break;
                }
            case AIChaseBehaviour.Flee:
                {
                    float t = 0f;
                    while (t < 1f)
                    {
                        t += Time.deltaTime;

                        MoveFromTarget();

                        yield return 0;
                    }
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
        while (true)
        {
            CurrentTarget = GetCurrentTarget();

            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = GetAvailableAbilityState();
                yield return StartCoroutine(WaitBehaviourRoutine());
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }
    
    #endregion
}
