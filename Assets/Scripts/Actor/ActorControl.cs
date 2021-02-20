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
        
        #if UNITY_EDITOR
        if(Input.GetKey(InputMap.Map["Console"]))
        {
            ConsoleInputUI.Instance.Show();
        }
        #endif

        if(Input.GetKey(InputMap.Map["Move Left"]))
        {
            if(CORE.Instance.MoveToHaltMode)
            {
                CurrentActor.HaltAbility();
            }

            CurrentActor.AttemptMoveLeft();
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]))
        {
            CurrentActor.AttemptMoveRight();

            if (CORE.Instance.MoveToHaltMode)
            {
                CurrentActor.HaltAbility();
            }
        }

        if (Input.GetKey(InputMap.Map["Move Up"]))
        {
            CurrentActor.AttemptMoveUp();

            if (CORE.Instance.MoveToHaltMode)
            {
                CurrentActor.HaltAbility();
            }
        }
        else if (Input.GetKey(InputMap.Map["Move Down"]))
        {
            CurrentActor.AttemptMoveDown();

            if (CORE.Instance.MoveToHaltMode)
            {
                CurrentActor.HaltAbility();
            }
        }

        if (Input.GetKeyDown(InputMap.Map["Jump"]) || Input.GetKeyDown(InputMap.Map["Move Up"]))
        {
            CurrentActor.AttemptJump();

            if (CORE.Instance.MoveToHaltMode)
            {
                CurrentActor.HaltAbility();
            }
        }
        
        for(int i=0;i<5;i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Ability"+i]))
            {
                if (CORE.Instance.RepressToHaltMode)
                {
                    CurrentActor.HaltAbility();
                }

                CurrentActor.AttemptPrepareAbility(i);
            }


            if (CORE.Instance.LongPressMode)
            {
                if (Input.GetKeyUp(InputMap.Map["Ability" + i]))
                {
                    if (CurrentActor.State.IsPreparingAbility)
                    {
                        CurrentActor.HaltAbility(i);
                    }
                }
            }
        }
        
    }
}
