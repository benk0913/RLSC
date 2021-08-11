using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MachinemaModeListenerEntity : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnActive;

    [SerializeField]
    UnityEvent OnInactive;
    void Start()
    {
        CORE.Instance.SubscribeToEvent("MachinemaModeRefresh",RefreshVisibility);
        RefreshVisibility();
    }

    void RefreshVisibility()
    {
        if(CORE.IsMachinemaMode)
        {
            OnActive?.Invoke();
        }
        else
        {
            OnInactive?.Invoke();
        }
    }
}
