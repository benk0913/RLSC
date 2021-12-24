using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControlsUI : MonoBehaviour
{
    public bool IsMovingLeft;
    public bool IsMovingRight;

    public bool IsMovingUp;
    public bool IsMovingDown;

#if !UNITY_ANDROID && !UNITY_IOS
    void Awake()
    {
        this.gameObject.SetActive(false);   
    }
#endif

    void Update()
    {
        if(IsMovingLeft)
        {
            CORE.PlayerActor.ActorEntity.AttemptMoveLeft();
        }
        else if(IsMovingRight)
        {
            CORE.PlayerActor.ActorEntity.AttemptMoveRight();
        }

        if(IsMovingUp)
        {
            CORE.PlayerActor.ActorEntity.AttemptMoveUp();
            CORE.PlayerActor.ActorEntity.AttemptJump();
        }
        else if(IsMovingDown)
        {
            CORE.PlayerActor.ActorEntity.AttemptMoveDown();
        }
        
    }

    public void TouchMoveLeft(bool state)
    {
        IsMovingLeft = state;
    }

    public void TouchMoveRight(bool state)
    {
        IsMovingRight = state;
    }

    public void TouchMoveUp(bool state)
    {
        IsMovingUp = state;
    }

    public void TouchMoveDown(bool state)
    {
        IsMovingDown = state;
    }


}
