using UnityEngine;
using UnityEngine.Events;

public class AbilitySlotDraggableUI : AbilitySlotUI
{
    [SerializeField]
    public UnityEvent OnSelect;

    [SerializeField]
    public UnityEvent OnDeselect;

    [SerializeField]
    public UnityEvent OnHover;

    [SerializeField]
    public UnityEvent OnUnhover;

    public void Hover()
    {
        OnHover?.Invoke();
    }

    public void Unhover()
    {
        OnUnhover?.Invoke();
    }

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    protected override void Update()
    {
        //Dont delete, this is done to terminate the inherited update.
    }

}
