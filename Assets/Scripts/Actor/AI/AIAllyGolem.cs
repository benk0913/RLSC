using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAllyGolem : AIAllyMob
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
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name == "AllyMobExistenceTimerX2" && x.CurrentCD <= 0f);

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
            x => x.CurrentAbility.name != "AllyMobExistenceTimerX2"
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
    


    public override Actor GetCurrentTarget()
    {

        if (ChaseBehaviour == AIChaseBehaviour.Chase && NoticeDistance > 0f && Asleep) //TODO Should probably replace with a less performance heavy implementation
        {
            if(CORE.Instance.Room.Actors.Find(X =>
                X.isMob
                && X.ActorEntity != null 
                && (Vector2.Distance(X.ActorEntity.transform.position, transform.position) < NoticeDistance || Act.State.Data.hp < Act.State.Data.MaxHP)
                && !X.ActorEntity.IsDead) != null)
            {
                Asleep = false;
                return null;
            }

            return null;
        }
        
        Actor target = CORE.Instance.Room.MostThreateningMob;
        if(target == null || target == this)
            return null;
        else 
            return target;
    }
}
