using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWitch : ActorAI
{
    public GameObject[] NPCPoints = new GameObject[0];

    protected override void Awake()
    {
        base.Awake();
        NPCPoints = GameObject.FindGameObjectsWithTag("NPC_Point");
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;
            GameObject SelectedPoint = null;
            bool canDoAbility = false;

            while (!canDoAbility)
            {
                if (SelectedAbility == null)
                {
                    
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Witch Dig" && x.CurrentCD <= 0f);
                }

                if (SelectedAbility == null)
                {
                    WaitBehaviour();
                }
                else
                {
                    if(SelectedPoint == null)
                    {
                        SelectedPoint = NPCPoints[Random.Range(0,NPCPoints.Length)];
                    }

                    float distFromPoint = transform.position.x - SelectedPoint.transform.position.x;
                    if(Mathf.Abs(distFromPoint)>0.15f)
                    {
                        if(distFromPoint > 0)
                        {
                            Act.AttemptMoveLeft();
                        }
                        else
                        {
                            Act.AttemptMoveRight();
                        }
                    }
                    else
                    {
                        SelectedPoint = null;
                        canDoAbility = true;
                    }
                }

                    
    

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
            canDoAbility = false;
            yield return 0;
        }
    }


    protected override void Start()
    {
        if (ChaseBehaviour == AIChaseBehaviour.Patrol)
        {
            System.Action patrolDirectionChangeAction = null;

            patrolDirectionChangeAction = () =>
            {
                if (this == null || !this.gameObject.activeInHierarchy)
                {
                    return;
                }

                if (patrolDirection && rhitLeft.collider)
                {
                    patrolDirection = false;
                }
                else if(!patrolDirection && rhitRight.collider != null)
                {
                    patrolDirection = true;
                }

                CORE.Instance.DelayedInvokation(1f, patrolDirectionChangeAction);
            };

            CORE.Instance.DelayedInvokation(1f, patrolDirectionChangeAction);

            patrolDirection = (Random.Range(0, 2) == 0);
            transform.position += transform.TransformDirection(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        }
    }


    bool CanSpawnMore
    {
        get
        {
            return CORE.Instance.Room.Actors.FindAll(x => x.isMob).Count < 15;
        }
    }

}
