using EdgeworldBase;
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

    
    [SerializeField]
    protected TextMeshProUGUI AbilityKeyLabel;


    [SerializeField]
    GameObject KeyLabelObject;

    
    public virtual void SetAbilityState(AbilityState abilityState = null, string abilityKeyText = "")
    {

        CurrentAbility = abilityState;

        if(!string.IsNullOrEmpty(abilityKeyText))
        {
            KeyLabelObject.SetActive(true);
            AbilityKeyLabel.text = abilityKeyText.Replace("Alpha","");
        }
        else
        {
            KeyLabelObject.SetActive(false);
            AbilityKeyLabel.text ="";
        }

        if(CurrentAbility == null || CurrentAbility.CurrentAbility == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = CurrentAbility.CurrentAbility.Icon;

        string tooltipString = "";

        string abilityName = abilityState.CurrentAbility.name;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(abilityState.CurrentAbility.name, out abilityName);

        string abilityDesc = abilityState.CurrentAbility.Description;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(abilityState.CurrentAbility.Description, out abilityDesc);

        tooltipString += "<color=" + Colors.COLOR_HIGHLIGHT + ">"+abilityName+"</color>";
        tooltipString += System.Environment.NewLine + abilityDesc;

        // TODO do we want detailed tooltips?
        // tooltipString += System.Environment.NewLine + "CASTING TIME: "+abilityState.CurrentAbility.CastingTime;
        // tooltipString += System.Environment.NewLine + "COOLDOWN: " + abilityState.CurrentAbility.CD;

        // if (abilityState.CurrentAbility.OnExecuteParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_HIGHLIGHT + ">On Execute</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnExecuteParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }

        // if (abilityState.CurrentAbility.OnHitParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_HIGHLIGHT + ">On Hit</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnHitParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }

        // if (abilityState.CurrentAbility.OnMissParams.Count > 0)
        // {
        //     tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_HIGHLIGHT + ">On Miss</color>";
        //     foreach (AbilityParam param in abilityState.CurrentAbility.OnMissParams)
        //     {
        //         tooltipString += System.Environment.NewLine + param.Type.name + " | " + param.Targets.ToString() + " | " + param.Value;
        //     }
        // }


        if (abilityState.IsAbilityLocked)
        {
            tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_BAD + "> - UNLOCK AT LEVEL "+abilityState.UnlockLevel+" - </color>";
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
