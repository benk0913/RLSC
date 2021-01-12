using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AbilitySlotDraggableUI : AbilitySlotUI,IPointerEnterHandler,IPointerDownHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        AbilitiesUI.Instance.MouseEnter(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AbilitiesUI.Instance.MouseClick(this);
    }
}
