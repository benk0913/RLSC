using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisplayCharacterUI : MonoBehaviour//, IPointerClickHandler
{
    public Action OnClick;

    public Actor AttachedCharacter;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    GameObject SelectionObject;

    public void SetInfo(Action onClick = null)
    {
        OnClick = onClick;
        GetComponent<Button>().enabled = true;
        TooltipTarget.Text = "Select Character: " + AttachedCharacter.State.Data.name;
        Deselect();
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    OnClick?.Invoke();
    //}

    public void InvokeOnClick()
    {
        OnClick?.Invoke();
    }

    public void Select()
    {
        SelectionObject.SetActive(true);
    }

    public void Deselect()
    {
        SelectionObject.SetActive(false);
    }
}
