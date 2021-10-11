using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGhost : ActorAI
{
    public GameObject EnragedEffect;



    protected override void Update()
    {
        base.Update();

        if (Act.State.Data.hp < Act.State.Data.MaxHP / 2f)
        {
            if (!EnragedEffect.activeInHierarchy)
            {
                EnragedEffect.SetActive(true);
            }
        }
        else
        {
            if (EnragedEffect.activeInHierarchy)
            {
                EnragedEffect.SetActive(false);
            }
        }
    }

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return 0;
        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (Act.State.Data.hp < Act.State.Data.MaxHP / 2f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Ghost Possess" && x.CurrentCD <= 0f);

                    if (SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Ghost Pull" && x.CurrentCD <= 0f);
                    }

                }
                else
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Ghost Possess" && x.CurrentCD <= 0f);

                    if (SelectedAbility == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Ghost Pull" && x.CurrentCD <= 0f);
                    }
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
