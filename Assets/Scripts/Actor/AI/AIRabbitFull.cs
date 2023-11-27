using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRabbitFull : ActorAI
{

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Carrot Explosion" && x.CurrentCD <= 0f);

                if (SelectedAbility == null)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name.Contains("Hop") && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return new WaitForSeconds(Random.Range(0.1f,1f));
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

    protected override void MoveToTarget()
    {
        return;
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

                CORE.Instance.DelayedInvokation(2f, patrolDirectionChangeAction);
            };

            CORE.Instance.DelayedInvokation(2f, patrolDirectionChangeAction);

            patrolDirection = (Random.Range(0, 2) == 0);
            transform.position += transform.TransformDirection(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        }
    }

}
