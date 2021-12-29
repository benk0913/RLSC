using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWispRed : ActorAI
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

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "WispElectricRed" && x.CurrentCD <= 0f);

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

        float verticalDistance = Mathf.Abs(CurrentTarget.transform.position.y -  transform.position.y);
        float horizontalDistance  = Mathf.Abs(CurrentTarget.transform.position.x - transform.position.x);
        if(verticalDistance - horizontalDistance < 1f)//Not that high or not that low
        {
            if (CurrentTarget.transform.position.x > transform.position.x && !rhitRight)
            {
                Act.AttemptMoveRight();
            }
            else if (CurrentTarget.transform.position.x < transform.position.x && !rhitLeft)
            {
                Act.AttemptMoveLeft();
            }
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
