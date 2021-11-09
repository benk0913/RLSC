using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRabbit : ActorAI
{
    [SerializeField]
    protected float closestActorDistance;

    [SerializeField]
    protected float EscapeDistance = 10f;

    [SerializeField]
    protected float ThrowCarrotRange = 40f;

    [SerializeField]
    protected float ThinkingTime;

    protected void LateUpdate()
    {
        closestActorDistance = Mathf.Infinity;
        for(int i=0;i< CORE.Instance.Room.Actors.Count;i++)
        {
            if(CORE.Instance.Room.Actors[i].isMob)
            {
                continue;
            }

            float currentDist = Vector2.Distance(transform.position, CORE.Instance.Room.Actors[i].ActorEntity.transform.position);
            if (currentDist < closestActorDistance)
            {
                closestActorDistance = currentDist;
            }
        }
    }

    protected override IEnumerator UseAbilityRoutine(AbilityState selectedAbility)
    {
        float abilityTimeout = 5f;
        
        if (selectedAbility.CurrentAbility.name == "Throw Carrot" || selectedAbility.CurrentAbility.name == "Carrot Explosion" )
        {
            MoveToTarget();

            yield return 0;

            if (CurrentTarget == null)
            {
                yield break;
            }

            float rangeDist = 0f;
            while (rangeDist > ThrowCarrotRange)
            {
                abilityTimeout -= Time.deltaTime;
                MoveToTarget();
                

                if (abilityTimeout <= 0f)
                {
                    yield break;
                }


                yield return 0;

                if(CurrentTarget == null)
                {
                    yield break;
                }
                else
                {
                    rangeDist = Vector2.Distance(transform.position, CurrentTarget.transform.position);
                }
            }
            
            Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));

            yield return 0;
        }
        else if (selectedAbility.CurrentAbility.name == "Escape")
        {
            if(rhitRight)
            {
                Act.AttemptMoveRight();
            }
            else if(rhitLeft)
            {
                Act.AttemptMoveLeft();
            }
            else
            {
                if(CurrentTarget.transform.position.x < transform.position.x)
                {
                    Act.AttemptMoveLeft();
                }
                else
                {
                    Act.AttemptMoveRight();
                }
            }
            
            Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));

            yield return 0;
        }

        yield return new WaitForSeconds(selectedAbility.CurrentAbility.CastingTime);

        yield return 0;
    }



    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;
            
            yield return new WaitForSeconds(ThinkingTime);

            while (SelectedAbility == null)
            {
                if(closestActorDistance < EscapeDistance)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Escape" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Carrot Explosion" && x.CurrentCD <= 0f);
                    }

                    if(SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Throw Carrot" && x.CurrentCD <= 0f);
                    }
                    
                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Carrot Explosion" && x.CurrentCD <= 0f);

                    if(SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Throw Carrot" && x.CurrentCD <= 0f);
                    }
                }

                if(SelectedAbility != null)
                    CORE.Instance.LogMessageError("SELECTED " + SelectedAbility.CurrentAbility.name);

                WaitBehaviour();

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

}
