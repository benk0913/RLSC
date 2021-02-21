using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISmallMush : ActorAI
{
    Actor _currentTarget;
    public override Actor GetCurrentTarget()
    {
        if(_currentTarget == null)
        {
            ActorData target = CORE.Instance.Room.Actors.Find(x=>x.classJob == "ActorMushroomBig");

            if(target == null)
            {
                // purposely don't cache, to keep looking for big mush.
                return null;
            }

            _currentTarget = target.ActorEntity;
        }

        return _currentTarget;
    }


    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = GetAvailableAbilityState();
                
                yield return StartCoroutine(MushroomPatrolRoutine());
            }
            
            yield return new WaitForSeconds(Random.Range(0f,0.2f));

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
            

            yield return 0;
        }
    }

    private IEnumerator MushroomPatrolRoutine()
    {

        float t=Random.Range(0.5f,1.5f);
        while(t>0f)
        {
            t-= Time.deltaTime;
            
            if(patrolDirection)
            {
                Act.AttemptMoveLeft();
            }
            else
            {
                Act.AttemptMoveRight();
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

}
