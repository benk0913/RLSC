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
                if ((Act.State.ExecutingAbilityCollider == null || !Act.State.ExecutingAbilityCollider.gameObject.activeInHierarchy)//Am I not already executing and am I below 70% HP?
                    && Act.State.Data.hp < Act.State.Data.MaxHP *0.7f)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FruitBossSpit" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "FruitBossSpit2" && x.CurrentCD <= 0f);
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
