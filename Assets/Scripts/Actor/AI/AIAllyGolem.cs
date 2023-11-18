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
                if (!Act.State.Data.states.ContainsKey("Invulnerable"))
                {
                    yield return new WaitForSeconds(1f);
                    SelectedAbility = Act.State.Abilities.Find(x => x.CurrentAbility.name.Contains("AllyMobExistenceTimer") && x.CurrentCD <= 0f);

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

    public override Actor GetCurrentTarget()
    {
        if(_cachedCurrentTarget == null || _cachedCurrentTarget.IsDead || !_cachedCurrentTarget.State.Data.isMob)
        {
            List<ActorData> roomMobs = CORE.Instance.Room.Actors.FindAll(x=>!x.ActorEntity.IsDead && x.isMob && x.ActorEntity != Act);

            if(roomMobs != null && roomMobs.Count > 0)
            {
                float minDistance = Mathf.Infinity;
                foreach(ActorData mob in roomMobs)
                {
                    float dist = Vector2.Distance(transform.position,mob.ActorEntity.transform.position);
                    if(dist < minDistance)
                    {
                        minDistance = dist;
                        _cachedCurrentTarget = mob.ActorEntity;
                    }
                }
            }
        }

        return _cachedCurrentTarget;
    }

    Actor _cachedCurrentTarget;


    protected override AbilityState GetAvailableAbilityState()
    {
        List<AbilityState> abilities = new List<AbilityState>();

        abilities.AddRange(Act.State.Abilities.FindAll(
            x => !x.CurrentAbility.name.Contains("AllyMobExistenceTimer")
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
