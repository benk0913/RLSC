using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPuffBunny : ActorAI
{
   
    
    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {

                if(Act.State.Data.hp < Act.State.Data.MaxHP *0.5f)
                {
                    this.ChaseBehaviour = AIChaseBehaviour.Flee;
                    
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Puff Bunny Spooked" && x.CurrentCD <= 0f);
                }
                else
                {
                    this.ChaseBehaviour = AIChaseBehaviour.Chase;
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
        }
    }
}
