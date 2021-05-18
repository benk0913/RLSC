using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISalamander : ActorAI
{


    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = GetAvailableAbilityState();

                WaitBehaviour();

                yield return 0;
            }
            
            yield return new WaitForSeconds(Random.Range(0f,0.2f));

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return new WaitForSeconds(3f);

            yield return 0;
        }
    }


}
