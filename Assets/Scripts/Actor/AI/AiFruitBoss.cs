using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFruitBoss : ActorAI
{
   
    
    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (Act.State.Data.hp < Act.State.Data.MaxHP *0.7f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FruitBossSpit" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            if(Random.Range(0,2) == 0)
            {
                Act.AttemptLookRight();
            }
            else
            {
                Act.AttemptLookLeft();
            }
            
            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
        }
    }
}
