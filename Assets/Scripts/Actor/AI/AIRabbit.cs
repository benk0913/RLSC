using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRabbit : ActorAI
{
    protected override IEnumerator UseAbilityRoutine(AbilityState selectedAbility)
    {
        float abilityTimeout = 5f;

        Debug.LogError("### -ABILITY-");
        if (selectedAbility.CurrentAbility.name == "Air Whip")
        {
            Debug.LogError("### Air Whip");
            while (Vector2.Distance(transform.position, CurrentTarget.transform.position) > 2f)
            {
                abilityTimeout -= Time.deltaTime;
                MoveToTarget();

                Debug.LogError("### Advancing towards target");

                if (abilityTimeout <= 0f)
                {
                    yield break;
                }


                yield return 0;
            }
            
            yield return 0;

            Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));
        }
        else if (selectedAbility.CurrentAbility.name == "Twirling")
        {
            Debug.LogError("### Twirling");

            if (Vector2.Distance(transform.position, CurrentTarget.transform.position) < 5f)
            {
                MoveFromTarget();

                yield return 0;

                Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));
            }
            else
            {
                yield break;
            }
        }
        else if (selectedAbility.CurrentAbility.name == "Wind Pull")
        {
            Debug.LogError("### Wind Pull!");
            Act.AttemptPrepareAbility(Act.State.Abilities.IndexOf(selectedAbility));
        }


        yield return new WaitForSeconds(selectedAbility.CurrentAbility.CastingTime);
    }



    protected override IEnumerator AIBehaviourRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            CurrentTarget = GetCurrentTarget();

            AbilityState SelectedAbility = null;

            while (SelectedAbility == null)
            {
                if(Act.State.Data.hp < CORE.Instance.Data.content.HP/2f)
                {
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Twirling");

                    if(SelectedAbility.CurrentCD > 0f)
                    {
                        SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "Air Whip");
                    }

                    if (SelectedAbility.CurrentCD > 0f)
                    {
                        SelectedAbility = null;
                    }
                }
                else
                {
                    SelectedAbility = GetAvailableAbilityState();
                }

                Debug.LogError("### Pending For ability...");
                yield return StartCoroutine(WaitBehaviourRoutine());
            }

            yield return StartCoroutine(UseAbilityRoutine(SelectedAbility));

            yield return 0;
        }
    }

}
