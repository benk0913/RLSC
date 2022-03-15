using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAllyMob : ActorAI
{
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
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "AllyMobExistenceTimer" && x.CurrentCD <= 0f);

                    if (SelectedAbility != null)
                    {
                        break;
                    }
                }
                else
                {
                    SelectedAbility = GetAvailableAbilityState();
                }
                
                WaitBehaviour();

                yield return 0;
            }

            yield return 0;

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

    protected override AbilityState GetAvailableAbilityState()
    {
        List<AbilityState> abilities = new List<AbilityState>();

        abilities.AddRange(Act.State.Abilities.FindAll(
            x => x.CurrentAbility.name != "AllyMobExistenceTimer"
            && x.CurrentCD <= 0 
            && CurrentTarget != null
            &&              
                        (x.CurrentAbility.AIViableRange == 0
                        || (x.CurrentAbility.AIViableRange > 0f && Vector2.Distance(transform.position, CurrentTarget.transform.position) < x.CurrentAbility.AIViableRange))));

        if(abilities.Count == 0)
        {
            return null;
        }

        return abilities[Random.Range(0, abilities.Count)];
    }
    
}
