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
            yield return new WaitForSeconds(10f);
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

                if (Random.Range(0,2) == 0)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "ImmobilizeBubble" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "ImmobilizeBubble2" && x.CurrentCD <= 0f);
                    }
                }
                else
                {
                    if (LeftCaterPillar)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CaterBlowLeft" && x.CurrentCD <= 0f);
                    }
                    else
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CaterBlow" && x.CurrentCD <= 0f);
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
