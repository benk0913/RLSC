using UnityEngine;
using UnityEngine.Events;

public class SelectionHandlerUI: MonoBehaviour
{
    public UnityEvent OnEnterEvent = new UnityEvent();
    public UnityEvent OnExitEvent = new UnityEvent();
}