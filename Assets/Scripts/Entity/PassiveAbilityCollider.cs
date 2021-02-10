using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbilityCollider : HitCollider
{
    [SerializeField]
    public float Interval = 3f;

    [SerializeField]
    protected float CurrentTime = 0f;

    public List<Actor> ActorsContained = new List<Actor>();

    protected void Update()
    {
        if(CurrentTime > 0f)
        {
            CurrentTime -= Time.deltaTime;
        }
        else
        {
            if (!ActorSource.IsImpassive)
            {
                CurrentTime = Interval;

                foreach (Actor actorVictim in ActorsContained)
                    AttemptHitAbility(actorVictim);
            }
        }
    }

    public override void SetInfo(Ability abilitySource, Actor actorSource)
    {
        base.SetInfo(abilitySource, actorSource);

        ActorsContained.Clear();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Actor")
        {
            Actor actorVictim = other.GetComponent<Actor>();

            ActorsContained.Add(actorVictim);

            if (!ActorSource.IsImpassive)
            {
                AttemptHitAbility(actorVictim);
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


}
