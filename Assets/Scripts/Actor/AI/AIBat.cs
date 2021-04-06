using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBat : ActorAI
{

    public float ChaseDistanceMax;

    protected override void Start()
    {
        base.Start();
        ChaseDistance = Random.Range(ChaseDistance, ChaseDistanceMax);
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                

                if (!Act.State.Data.states.ContainsKey("Flying"))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "BatFlight" && x.CurrentCD <= 0f);

                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }
    

    protected override void MoveToTarget()
    {
        if (CurrentTarget == null)
        {
            return;
        }

        if (CurrentTarget.transform.position.x > transform.position.x)
        {
            Act.AttemptMoveRight();
        }
        else if (CurrentTarget.transform.position.x < transform.position.x)
        {
            Act.AttemptMoveLeft();
        }

        if (CurrentTarget.transform.position.y > transform.position.y)
        {

            Act.AttemptMoveUp();
        }
        else if (CurrentTarget.transform.position.y < transform.position.y)
        {
            Act.AttemptMoveDown();
        }

    }
}
