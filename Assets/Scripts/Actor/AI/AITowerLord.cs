using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITowerLord : ActorAI
{
   
    
    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if ((Act.State.ExecutingAbilityCollider == null || !Act.State.ExecutingAbilityCollider.gameObject.activeInHierarchy)//Am I not already executing and am I below 70% HP?
                     && Act.State.Data.hp < Act.State.Data.MaxHP *0.6f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Tower Lord Walls" && x.CurrentCD <= 0f);
                }

                if(SelectedAbility == null)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Tower Lord Curse" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Tower Lord Blame" && x.CurrentCD <= 0f);
                    }
                }


                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
        }
    }
}
