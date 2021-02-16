using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISnake : ActorAI
{
    public bool HasEnemiesToLeft
    {
        get
        {
            foreach(ActorData actor in CORE.Instance.Room.Actors)
            {
                if (!actor.isMob && actor.ActorEntity != null && !actor.ActorEntity.IsDead && actor.ActorEntity.transform.position.x < transform.position.x)
                {
                    
                    return true;
                }
            }

            return false;
        }
    }
    
    public bool HasEnemiesToRight
    {
        get
        {
            foreach(ActorData actor in CORE.Instance.Room.Actors)
            {
                if (!actor.isMob && actor.ActorEntity != null && !actor.ActorEntity.IsDead && actor.ActorEntity.transform.position.x > transform.position.x)
                {
                    
                    return true;
                }
            }

            return false;
        }
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {

                if (Random.Range(0, 4) == 0)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Hypno 1" && x.CurrentCD <= 0f);
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite 1" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite 3" && x.CurrentCD <= 0f);
                    }
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

    protected override void MoveToTarget()
    {
        int random = Random.Range(0, 2);

        if (!HasEnemiesToLeft)
        {
            Act.AttemptLookRight();
        }
        else if (!HasEnemiesToRight)
        {
            Act.AttemptLookLeft();
        }
        else if (Random.Range(0, 2) == 0)
        {
            Act.AttemptLookRight();
        }
        else
        {
            Act.AttemptLookLeft();
        }
    }
}
