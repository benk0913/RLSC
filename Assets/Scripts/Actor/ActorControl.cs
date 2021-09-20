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
        
        if(Input.GetKeyDown(InputMap.Map["Console"]) || Input.GetKeyDown(InputMap.Map["Console Alt"]))
        {
            ConsoleInputUI.Instance.EnterPressed();
        }

        if(Input.GetKey(InputMap.Map["Move Left"])|| Input.GetKey(InputMap.Map["Secondary Move Left"]))
        {
            ConsoleInputUI.Instance.HideIfEmpty();
            CurrentActor.AttemptMoveLeft();
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]) || Input.GetKey(InputMap.Map["Secondary Move Right"]))
        {
            ConsoleInputUI.Instance.HideIfEmpty();
            CurrentActor.AttemptMoveRight();
        }

        if (Input.GetKey(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]))
        {
            CurrentActor.AttemptMoveUp();
        }
        else if (Input.GetKey(InputMap.Map["Move Down"])|| Input.GetKey(InputMap.Map["Secondary Move Down"]))
        {
            CurrentActor.AttemptMoveDown();
        }

        if (Input.GetKeyDown(InputMap.Map["Jump"]) || Input.GetKeyDown(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]))
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
        
        for(int i = 1; i < 9; i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Emote "+i]))
            {
                CurrentActor.AttemptEmote(i);
            }
        }

    }
}
