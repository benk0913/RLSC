using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorControl : MonoBehaviour
{
    

    [SerializeField]
    Actor CurrentActor;

    float internalJumpCooldown = 0f;

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

        if(internalJumpCooldown > 0f)
        {
            internalJumpCooldown -= 1f * Time.deltaTime;
        }

        if(Input.GetKey(InputMap.Map["Move Left"]))
        {
            CurrentActor.AttemptMoveLeft();
        }
        else if(Input.GetKey(InputMap.Map["Move Right"]))
        {
            CurrentActor.AttemptMoveRight();
        }

        if (Input.GetKeyDown(InputMap.Map["Jump"]))
        {
            if (internalJumpCooldown > 0f)
            {
                return;
            }

            CurrentActor.AttemptJump();
            internalJumpCooldown = 1f;
        }
        
        for(int i=0;i<5;i++)
        {
            if (Input.GetKeyDown(InputMap.Map["Ability"+i]))
            {
                CurrentActor.AttemptPrepareAbility(i);
            }
        }
        
    }
}
