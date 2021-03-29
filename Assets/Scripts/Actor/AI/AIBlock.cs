using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBlock : ActorAI
{
  
    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (Act.State.Data.hp < Act.State.Data.MaxHP /2f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "TurtleSmash" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "BlockEnrageHaste" && x.CurrentCD <= 0f);
                    }

                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "TurtleSmash" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }
            
            yield return new WaitForSeconds(Random.Range(0f,0.2f));

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
            

            yield return 0;
        }
    }



    

}
