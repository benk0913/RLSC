using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MobileEntity : MonoBehaviour
{
    public UnityEvent OnIsMobile;
    public UnityEvent OnIsNotMobile;

    void OnEnable()
    {
        #if UNITY_ANDROID || UNITY_IOS
        OnIsMobile?.Invoke();
        #else
        OnIsNotMobile?.Invoke();
        #endif
    }
}
