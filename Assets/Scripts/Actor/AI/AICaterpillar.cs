using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICaterpillar : ActorAI
{
    public bool LeftCaterPillar;

    protected override void Start()
    {
        if (LeftCaterPillar)
        {
            Act.AttemptLookRight();
        }
        else
        {
            Act.AttemptLookLeft();
        }

        base.Start();
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        if (LeftCaterPillar)
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (true)
        {
       
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {

                if (Act.State.Data.hp < Act.State.Data.MaxHP / 3f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CaterRejuv" && x.CurrentCD <= 0f);
                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CaterBlow" && x.CurrentCD <= 0f);
                }

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));
        }
    }

}
