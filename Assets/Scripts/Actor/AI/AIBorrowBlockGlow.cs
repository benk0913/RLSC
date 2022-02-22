using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBorrowBlockGlow : ActorAI
{


    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (Act.State.Data.hp < Act.State.Data.MaxHP /2f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "BorrowHolePoisonGlow" && x.CurrentCD <= 0f);

                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }
                
                if(Act.State.Data.hp < (Act.State.Data.MaxHP*0.4f))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FlipScreen" && x.CurrentCD <= 0f);
                    
                    if(SelectedAbility != null)
                    {
                        break;
                    }
                }

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Ghost Possess" && x.CurrentCD <= 0f);

                if(SelectedAbility != null)
                {
                    break;
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }


}
