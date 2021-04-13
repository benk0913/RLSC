using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlime : ActorAI
{

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (CanSpawnMore && Act.State.Data.hp < (Act.State.Data.MaxHP/2f))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Split Medium" && x.CurrentCD <= 0f);

                    if (SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Split Large" && x.CurrentCD <= 0f);
                    }
                }


                if (SelectedAbility == null && CanSpawnMore)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "SlimeMuck" && x.CurrentCD <= 0f);
                }

                if (SelectedAbility == null)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Hop" && x.CurrentCD <= 0f);
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


    bool CanSpawnMore
    {
        get
        {
            return CORE.Instance.Room.Actors.FindAll(x => x.isMob).Count < 15;
        }
    }

}
