using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplatformUIManager : MonoBehaviour
{


    public UnityEvent OnAndroid;
    public UnityEvent OnSteam;

    public static bool IsUniversalDropDown;
    public static bool IsUniversalJump;
    public static bool IsUniversalInteract;
    public static bool IsUniversalPickUp;

    public static bool IsUniversalLeft;

    public static bool IsUniversalRight;

    public static bool IsUniversalToggleChat;

    public static bool IsUniversalUp;
    public static bool IsUniversalDown;
    public SimpleTouchController TouchController;

    void Awake()
    {
        #if UNITY_ANDROID
        OnAndroid?.Invoke();
        this.transform.localScale *= 0.8f;
        #else
        OnSteam?.Invoke();
        #endif
    }

    void Update()
    {
        if(TouchController.GetTouchPosition.x > 0.05f)
        {
            IsUniversalRight = true;
        }
        else if(TouchController.GetTouchPosition.x < -0.05f)
        {
            IsUniversalLeft = true;
        }
         else if(TouchController.GetTouchPosition.y < -0.1f)
         {
             IsUniversalDown = true;
         }
         else if(TouchController.GetTouchPosition.y > 0.1f)
         {
             IsUniversalUp = true;
         }
    }

    void LateUpdate()
    {
        IsUniversalJump = false;
        IsUniversalInteract = false;
        IsUniversalDropDown= false;
        IsUniversalPickUp= false;
        IsUniversalLeft = false;
        IsUniversalRight = false;
        IsUniversalUp = false;
        IsUniversalDown = false;
        IsUniversalToggleChat = false; 
    }
    public void Jump()
    {
        IsUniversalJump = true;
        UnityAndroidVibrator.VibrateForGivenDuration(100);
    }

    public void DropDown()
    {
        IsUniversalDropDown = true;
    }

    public void PickUp()
    {
        IsUniversalPickUp = true;
        UnityAndroidVibrator.VibrateForGivenDuration(100);
    }

    public void Interact()
    {
        IsUniversalInteract = true;
        UnityAndroidVibrator.VibrateForGivenDuration(100);
    }

    public void Chat()
    {
        IsUniversalToggleChat = true;
        UnityAndroidVibrator.VibrateForGivenDuration(100);
    }
    
}
