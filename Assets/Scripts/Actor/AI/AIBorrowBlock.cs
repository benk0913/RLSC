using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBorrowBlock : ActorAI
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
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "BorrowHolePoison" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

}
