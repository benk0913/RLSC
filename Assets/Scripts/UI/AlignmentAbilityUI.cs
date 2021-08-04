using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;

public class AlignmentAbilityUI : MonoBehaviour, IPointerClickHandler
{
    AlignmentAbility CurrentAbility;

    [SerializeField]
    public Image AbilityIcon;

    [SerializeField]
    public TextMeshProUGUI AbilityLabel;

    [SerializeField]
    public UnityEvent OnActive;

    [SerializeField]
    public UnityEvent OnInactive;

    [SerializeField]
    public TooltipTargetUI TooltipTarget;

    public void SetInfo(AlignmentAbility ability)
    {
        CurrentAbility = ability;
        RefreshUI();
    }

    public void RefreshUI()
    {
        AbilityLabel.text = CurrentAbility.name;
        TooltipTarget.Text = CurrentAbility.Description;
        AbilityIcon.sprite = CurrentAbility.Icon;

        if(CurrentAbility.IsActive)
        {
            OnActive?.Invoke();
        }
        else
        {
            OnInactive?.Invoke();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!CurrentAbility.IsActive)
        {
            TopNotificationUI.Instance.Show(
                new TopNotificationUI.TopNotificationInstance("You will have to unlock this ability first!",Color.red,3f));
        }

        JSONNode data = new JSONClass();
        data["ability"] = CurrentAbility.name;
        SocketHandler.Instance.SendEvent("activate_alignment_ability",data);
    }
}
