using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilitySlotDraggableUI : AbilitySlotUI
{
    [SerializeField]
    public UnityEvent OnSelect;

    [SerializeField]
    public UnityEvent OnDeselect;

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }
    
    public void OnSelectionEnter()
    {
        Tooltip.ShowOnPosition(transform.position);
    }

    public void OnSelectionExit()
    {
        Tooltip.Hide();
    }

    public void OnSelection()
    {
        AbilitiesUI.Instance.SelectAbility(this);
    }

    protected override void Update()
    {
        //Dont delete, this is done to terminate the inherited update.
    }
}
