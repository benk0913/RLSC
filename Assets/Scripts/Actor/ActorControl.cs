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
        
        if(MultiplatformUIManager.IsUniversalToggleChat || Input.GetKeyDown(InputMap.Map["Console"]) || Input.GetKeyDown(InputMap.Map["Console Alt"]) || Input.GetButtonDown("Joystick 6") || Input.GetButtonDown("Joystick 7")|| Input.GetButtonDown("Joystick 9") ||  Input.GetButtonDown("Joystick 10"))
        {
            ConsoleInputUI.Instance.EnterPressed();
        }

        if(Input.GetKeyDown(InputMap.Map["Exit"]) && ConsoleInputUI.Instance.gameObject.activeInHierarchy)
        {
            ConsoleInputUI.Instance.Hide();
        }

        if(MultiplatformUIManager.IsUniversalLeft || Input.GetKey(InputMap.Map["Move Left"])|| Input.GetKey(InputMap.Map["Secondary Move Left"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") < 0f))
        {
            ConsoleInputUI.Instance.HideIfEmpty();
            CurrentActor.AttemptMoveLeft();
        }
        else if(MultiplatformUIManager.IsUniversalRight||Input.GetKey(InputMap.Map["Move Right"]) || Input.GetKey(InputMap.Map["Secondary Move Right"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Horizontal") > 0f))
        {
            ConsoleInputUI.Instance.HideIfEmpty();
            CurrentActor.AttemptMoveRight();
        }

        if (MultiplatformUIManager.IsUniversalUp ||Input.GetKey(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") > 0f))
        {
            CurrentActor.AttemptMoveUp();
        }
        else if (MultiplatformUIManager.IsUniversalDown||Input.GetKey(InputMap.Map["Move Down"])|| Input.GetKey(InputMap.Map["Secondary Move Down"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") < 0f))
        {
            CurrentActor.AttemptMoveDown();
        }

        if (MultiplatformUIManager.IsUniversalJump||MultiplatformUIManager.IsUniversalUp || Input.GetKey(InputMap.Map["Jump"]) || Input.GetKey(InputMap.Map["Move Up"]) || Input.GetKey(InputMap.Map["Secondary Move Up"]) || (CORE.Instance.IsUsingJoystick && Input.GetAxis("Vertical") > 0.8f))
        {
            CurrentActor.AttemptJump();
        }


        if(Input.GetButtonDown("Joystick 0"))
        {
        }
        else if(Input.GetButtonDown("Joystick 2"))
        {
            CurrentActor.AttemptPrepareAbility(2);
        }
        else if (Input.GetButtonDown("Joystick 1"))
        {
            CurrentActor.AttemptPrepareAbility(1);
            ConsoleInputUI.Instance.Hide();
        }
        else if (Input.GetButtonDown("Joystick 3"))
        {
            CurrentActor.AttemptPrepareAbility(0);
        }
        else if (Input.GetButtonDown("Joystick 4"))
        {
            CurrentActor.AttemptPrepareAbility(3);
        }
        else if (Input.GetButtonDown("Joystick 5"))
        {
            CurrentActor.AttemptPrepareAbility(4);
        }


            int abilitiesLength = CurrentActor.State.Data.abilities.Count;
        for(int i = 0; i < abilitiesLength; i++)
        {
            // if (Input.GetKeyDown(InputMap.Map["Ability"+i]))
            // {
            //     CurrentActor.AttemptPrepareAbility(i);
            // }

            if (Input.GetKey(InputMap.Map["Ability"+i]))
            {
                AbilityState abilityState = CurrentActor.State.Abilities[i];

                if(abilityState.CurrentCastingTime <= 0f && abilityState.CurrentCD <= 0)
                {
                    CurrentActor.AttemptPrepareAbility(i);
                }
            }
        }
        
        for(int i = 1; i < 10; i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Emote "+i]))
            {
                CurrentActor.AttemptEmote(i);
            }
        }

    }
}
