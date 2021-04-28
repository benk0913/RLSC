using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorControl : MonoBehaviour
{
    

    [SerializeField]
    Actor CurrentActor;

    private void Reset()
    {
        CurrentActor = GetComponent<Actor>();
    }

    private void Update()
    {
        if(CurrentActor.ControlSource != ControlSourceType.Player)
        {
            return;
        }
        
        if(Input.GetKeyDown(InputMap.Map["Console"]))
        {
            ConsoleInputUI.Instance.Show();
        }

        if(Input.GetKey(InputMap.Map["Move Left"]))
        {
            CurrentActor.AttemptMoveLeft();
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]))
        {
            CurrentActor.AttemptMoveRight();
        }

        if (Input.GetKey(InputMap.Map["Move Up"]))
        {
            CurrentActor.AttemptMoveUp();
        }
        else if (Input.GetKey(InputMap.Map["Move Down"]))
        {
            CurrentActor.AttemptMoveDown();
        }

        if (Input.GetKeyDown(InputMap.Map["Jump"]) || Input.GetKeyDown(InputMap.Map["Move Up"]))
        {
            CurrentActor.AttemptJump();
        }
        
        int abilitiesLength = CurrentActor.State.Data.abilities.Count;
        for(int i = 0; i < abilitiesLength; i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Ability"+i]))
            {
                CurrentActor.AttemptPrepareAbility(i);
            }
        }
        
    }
}
