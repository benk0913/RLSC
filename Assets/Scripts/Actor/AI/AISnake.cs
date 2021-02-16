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
                if (actor.ActorEntity != null && actor.isCharacter && actor.ActorEntity.transform.position.x < transform.position.x)
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
            foreach (ActorData actor in CORE.Instance.Room.Actors)
            {
                if (actor.ActorEntity != null && actor.isCharacter && actor.ActorEntity.transform.position.x > transform.position.x)
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
                    if (!HasEnemiesToLeft)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Hypno 2" && x.CurrentCD <= 0f);
                    }
                    else if (!HasEnemiesToRight)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Hypno 1" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        int rndDir = Random.Range(1, 3);
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Hypno " + rndDir && x.CurrentCD <= 0f);
                    }
                }
                else
                {
                    if (!HasEnemiesToLeft)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite 2" && x.CurrentCD <= 0f);
                        }
                        else
                        {
                            SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite 4" && x.CurrentCD <= 0f);
                        }
                    }
                    else if (!HasEnemiesToRight)
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
                    else
                    {

                        int rndDir = Random.Range(1, 5);
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite " + rndDir && x.CurrentCD <= 0f);
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
        return;
    }
}
