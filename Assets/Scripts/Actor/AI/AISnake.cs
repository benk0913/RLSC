using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISnake : ActorAI
{

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {

                if (Random.Range(0, 2) == 0)
                {
                    int rndDir = Random.Range(1, 5);
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Bite "+rndDir && x.CurrentCD <= 0f);
                }
                else
                {
                    int rndDir = Random.Range(1, 3);
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Snake Hypno " + rndDir && x.CurrentCD <= 0f);
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
