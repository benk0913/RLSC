using UnityEngine;
using UnityEngine.Events;

public class SelectionHandlerUI: UnityEngine.UI.Selectable
{
    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;
    public UnityEvent OnSelectEvent;
}