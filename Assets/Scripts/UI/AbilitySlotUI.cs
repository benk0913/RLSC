using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public AbilityState CurrentAbility;

    [SerializeField]
    protected Image IconImage;

    [SerializeField]
    protected Image CooldownImage;

    [SerializeField]
    protected TextMeshProUGUI CooldownLabel;

    [SerializeField]
    protected Image CastingCooldownImage;

    [SerializeField]
    protected TextMeshProUGUI CastingCooldownLabel;

    [SerializeField]
    protected TooltipTargetUI Tooltip;

    [SerializeField]
    GameObject AbilityLock;


    public virtual void SetAbilityState(AbilityState abilityState = null)
    {

        CurrentAbility = abilityState;

        if(CurrentAbility == null || CurrentAbility.CurrentAbility == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = CurrentAbility.CurrentAbility.Icon;

        string tooltipString = "";

        tooltipString += "<color=yellow>"+abilityState.CurrentAbility.name+"</color>";
        tooltipString += System.Environment.NewLine + abilityState.CurrentAbility.Description;

        // TODO do we want detailed tooltips?
        // tooltipString += System.Environment.NewLine + "CASTING TIME: "+abilityState.CurrentAbility.CastingTime;
        // tooltipString += System.Environment.NewLine + "COOLDOWN: " + abilityState.CurrentAbility.CD;

        // if (abilityState.CurrentAbility.OnExecuteParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=yellow>On Execute</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnExecuteParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }

        // if (abilityState.CurrentAbility.OnHitParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=yellow>On Hit</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnHitParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }

        // if (abilityState.CurrentAbility.OnMissParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=yellow>On Miss</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnMissParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }


        if (abilityState.IsAbilityLocked)
        {
            tooltipString += System.Environment.NewLine + "<color=red> - LOCKED - </color>";
            AbilityLock.SetActive(true);
        }
        else
        {
            AbilityLock.SetActive(false);
        }

        Tooltip.SetTooltip(tooltipString);
    }

    protected virtual void Update()
    {
        if(CurrentAbility == null)
        {
            return;
        }

        if(CurrentAbility.CurrentCD <= 0 && CooldownImage.gameObject.activeInHierarchy)
        {
            CooldownImage.gameObject.SetActive(false);
        }
        else if(CurrentAbility.CurrentCD > 0)
        {
            if (!CooldownImage.gameObject.activeInHierarchy)
            {
                CooldownImage.gameObject.SetActive(true);
            }

            CooldownImage.fillAmount = CurrentAbility.CurrentCD / CurrentAbility.CurrentAbility.CD;
            CooldownLabel.text = Mathf.RoundToInt(CurrentAbility.CurrentCD).ToString();
        }

        if (CurrentAbility.CurrentCastingTime <= 0 && CastingCooldownImage.gameObject.activeInHierarchy)
        {
            CastingCooldownImage.gameObject.SetActive(false);
        }
        else if (CurrentAbility.CurrentCastingTime > 0)
        {
            if (!CastingCooldownImage.gameObject.activeInHierarchy)
            {
                CastingCooldownImage.gameObject.SetActive(true);
            }

            CastingCooldownImage.fillAmount = CurrentAbility.CurrentCastingTime / CurrentAbility.CurrentAbility.CastingTime;
            CastingCooldownLabel.text = Mathf.RoundToInt(CurrentAbility.CurrentCastingTime).ToString();
        }
    }
}
