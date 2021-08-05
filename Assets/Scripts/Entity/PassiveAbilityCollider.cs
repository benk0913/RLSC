using System.Collections.Generic;
using UnityEngine;

public class PassiveAbilityCollider : HitCollider
{
    public float Interval = 2f;

    public Dictionary<Actor, float> ActorsTimers = new Dictionary<Actor, float>();

    public HashSet<Actor> ActorsContained = new HashSet<Actor>();

    protected void Update()
    {
        List<Actor> actors = new List<Actor>(ActorsTimers.Keys);
        foreach (Actor actorVictim in actors)
        {
            if(ActorsTimers[actorVictim] > 0f)
            {
                ActorsTimers[actorVictim] -= Time.deltaTime;
            }
            else
            {
                // Actors that are removed still have a timer in case they come back, so they won't get hit twice.
                // Once the timer is done, remove gone actors.
                if (!ActorsContained.Contains(actorVictim))
                {
                    ActorsTimers.Remove(actorVictim);
                }
                else if (AttemptHitAbility(actorVictim))
                {
                    ActorsTimers[actorVictim] = Interval;
                }
            }
        }
    }

    public override void SetInfo(Ability abilitySource, Actor actorSource, HitCollider parentCollider = null)
    {
        base.SetInfo(abilitySource, actorSource);

        ActorsContained.Clear();
        ActorsTimers.Clear();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Actor")
        {
            Actor actorVictim = other.GetComponent<Actor>();

            //TODO Added null check, but didnt solve the issue from the start.
            if (actorVictim == null)
            {
                Debug.LogError("No Actor component on " + other.gameObject.name);

                return;
            }

            ActorsContained.Add(actorVictim);
            
            if (!ActorsTimers.ContainsKey(actorVictim))
            {
                ActorsTimers[actorVictim] = 0;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Actor")
        {
            Actor actorVictim = other.GetComponent<Actor>();
            
            if(ActorsContained.Contains(actorVictim))
                ActorsContained.Remove(actorVictim);
        }
    }
    
    protected override bool CanHitActor(Actor actorVictim)
    {
        return !ActorSource.IsImpassive && base.CanHitActor(actorVictim);
    }
}
