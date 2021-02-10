using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlime : ActorAI
{

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (Act.State.Data.hp < (Act.State.Data.MaxHP*0.2f))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Split" && x.CurrentCD <= 0f);

                    if (SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Hop" && x.CurrentCD <= 0f);
                    }
                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Slime Hop" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

}
