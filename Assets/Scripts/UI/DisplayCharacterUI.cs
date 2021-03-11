using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayCharacterUI : MonoBehaviour, IPointerClickHandler
{
    public Action OnClick;

    public Actor AttachedCharacter;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    public void SetInfo(Action onClick = null)
    {
        OnClick = onClick;
        TooltipTarget.Text = "Select Character: " + AttachedCharacter.State.Data.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }


}
