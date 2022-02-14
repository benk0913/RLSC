using System;
using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using UnityEngine;
using UnityEngine.Events;

public class MultiplatformUIManager : MonoBehaviour
{
    public static MultiplatformUIManager Instance;

    public UnityEvent OnAndroid;
    public UnityEvent OnSteam;
    
    public static InteractableEntity CurrentInteractable;
    public static InteractableEntity CurrentItem;

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

    public GameObject CanInteractIndicator;
    public GameObject CanPickUpIndicator;

    public bool PickUpButtonIsDOWN;

    void Awake()
    {
        Instance = this;
        #if UNITY_ANDROID
        OnAndroid?.Invoke();
        this.transform.localScale *= 0.85f;
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
        if(TouchController.GetTouchPosition.y < -0.25f)
        {
            IsUniversalDown = true;
        }
        else if(TouchController.GetTouchPosition.y > 0.25f)
        {
            IsUniversalUp = true;
        }
    }

    void LateUpdate()
    {
        IsUniversalJump = false;
        IsUniversalInteract = false;
        IsUniversalDropDown= false;
        
        if(!PickUpButtonIsDOWN)
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
        AudioControl.Instance.Play("IconEnter");
    }

    public void DropDown()
    {
        IsUniversalDropDown = true;
    }

    public void PickUp()
    {
        PickUpButtonIsDOWN = true;
        IsUniversalPickUp = true;
        AudioControl.Instance.Play("IconEnter");
    }

    public void PickUpRelease()
    {
        PickUpButtonIsDOWN = false;
    }

    public void Interact()
    {
        IsUniversalInteract = true;
        AudioControl.Instance.Play("IconEnter");
    }

    public void Chat()
    {
        IsUniversalToggleChat = true;
    }

    internal static void SetCurrentInteractable(InteractableEntity interactable)
    {
        CurrentInteractable = interactable;
        Instance.CanInteractIndicator.SetActive(CurrentInteractable != null);
        
    }

    internal static void SetCurrentItem(InteractableEntity item)
    {
        CurrentItem = item;
        Instance.CanPickUpIndicator.SetActive(CurrentItem != null);
    }
}
