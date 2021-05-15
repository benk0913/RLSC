using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICat : ActorAI
{
    public bool TheSecondCat = false;

    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if (!Act.State.Data.states.ContainsKey("Shielded"))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatShielding" && x.CurrentCD <= 0f);

                    if (SelectedAbility != null)
                    {
                        break;
                    }
                }
                
                if (Act.State.Data.hp < Act.State.Data.MaxHP / 2f &&  !Act.State.Data.states.ContainsKey("Threat Trickery"))
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatEnrageThreat" && x.CurrentCD <= 0f);
                    if (SelectedAbility != null)
                    {
                        break;
                    }

                    if (Act.State.Buffs.Find(x => x.CurrentBuff.name == "CatEnrage") == null)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatEnrage" && x.CurrentCD <= 0f);
                        if (SelectedAbility != null)
                        {
                            break;
                        }
                    }

                    
                }

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatShieldBash" && x.CurrentCD <= 0f);

                if (SelectedAbility != null)
                {
                    break;
                }

                SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "CatSmash" && x.CurrentCD <= 0f);

                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

    public override Actor GetCurrentTarget()
    {
        if (TheSecondCat)
        {
            if (Act.State.Data.states.ContainsKey("Threat Trickery"))
            {
                return CORE.Instance.Room.LeastThreatheningActor;
                
            }

            return base.GetCurrentTarget();
        }
        else
        {
            if (Act.State.Data.states.ContainsKey("Threat Trickery"))
            {
                return base.GetCurrentTarget();
            }

            return CORE.Instance.Room.LeastThreatheningActor;
        }

    }

}
