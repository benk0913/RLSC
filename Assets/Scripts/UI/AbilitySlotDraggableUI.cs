using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilitySlotDraggableUI : AbilitySlotUI,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
        AbilitiesUI.Instance.SetHover(this);
        OnHover?.Invoke();
    }

    public void Unhover()
    {
        OnUnhover?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unhover();
    }

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
        Hover();
    }

    public void OnSelectionExit()
    {
        Tooltip.Hide();
        Unhover();
    }

    public void OnSelection()
    {
        AbilitiesUI.Instance.SelectAbility();
    }

    protected override void Update()
    {
        //Dont delete, this is done to terminate the inherited update.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AbilitiesUI.Instance.SelectAbility();
    }
}
