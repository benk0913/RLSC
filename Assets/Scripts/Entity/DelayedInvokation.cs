using System;
using UnityEngine;
using UnityEngine.Events;

public class DelayedInvokation : MonoBehaviour
{
    [SerializeField] public UnityEvent OnDelay;

    [SerializeField] private float Delay;

    private void OnEnable()
    {
        CORE.Instance.DelayedInvokation(Delay, OnDelay.Invoke);
    }
}
