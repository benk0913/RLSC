using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMudCreature : ActorAI
{



    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Mud Creature Chomp" && x.CurrentCD <= 0f);

                if (SelectedAbility == null)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Mud Creature Burp" && x.CurrentCD <= 0f);
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
