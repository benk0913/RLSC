using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitySlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
    GameObject AbilityMastery;

    [SerializeField]
    Animator Animer;

    
    [SerializeField]
    protected TextMeshProUGUI AbilityKeyLabel;


    [SerializeField]
    GameObject KeyLabelObject;

    public bool PressHold = false;
    public virtual void SetAbilityState(AbilityState abilityState = null, string abilityKeyText = "")
    {

        CurrentAbility = abilityState;
        
        #if UNITY_ANDROID || UNITY_IOS
            KeyLabelObject.SetActive(false);
        #else

        if(!string.IsNullOrEmpty(abilityKeyText))
        {
            KeyLabelObject.SetActive(true);
            AbilityKeyLabel.text = abilityKeyText.Replace("Alpha","");
            if(CORE.Instance.IsUsingJoystick)
            {
                AbilityKeyLabel.text = AbilityKeyLabel.text.Replace("1","Y");       
                AbilityKeyLabel.text = AbilityKeyLabel.text.Replace("2","B");       
                AbilityKeyLabel.text = AbilityKeyLabel.text.Replace("3","X");       
                AbilityKeyLabel.text = AbilityKeyLabel.text.Replace("4","LB");       
                AbilityKeyLabel.text = AbilityKeyLabel.text.Replace("5","RB");       
            }
        }
        else
        {
            KeyLabelObject.SetActive(false);
            AbilityKeyLabel.text ="";
        }
        
        #endif

        if(CurrentAbility == null || CurrentAbility.CurrentAbility == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = CurrentAbility.CurrentAbility.Icon;

        string tooltipString = "";
        

        tooltipString += "<color=" + Colors.COLOR_HIGHLIGHT + ">"+CORE.QuickTranslate(abilityState.CurrentAbility.name)+ "</color>";
        tooltipString += System.Environment.NewLine + CORE.QuickTranslate(abilityState.CurrentAbility.Description);
        if(!string.IsNullOrEmpty(abilityState.CurrentAbility.MasteryDescription))
            tooltipString += System.Environment.NewLine + CORE.QuickTranslate("Mastery")+": "+CORE.QuickTranslate(abilityState.CurrentAbility.MasteryDescription);
        tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_HIGHLIGHT + ">" + CORE.QuickTranslate("cooldown")+": "+abilityState.CurrentAbility.CD+" "+CORE.QuickTranslate("seconds")+".</color>";


        AbilityMastery.SetActive(abilityState.CurrentAbility.Mastery);

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


        if (abilityState.IsLevelLocked)
        {
            tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_BAD + "> - "+CORE.QuickTranslate("UNLOCK AT LEVEL")+" "+abilityState.UnlockLevel+" - </color>";
            AbilityLock.SetActive(true);
        }
        else if (abilityState.IsOrbLocked)
        {
            tooltipString += System.Environment.NewLine + "<color=" + Colors.COLOR_BAD + "> - "+CORE.QuickTranslate("LOCKED BY ORB")+" - </color>";
            AbilityLock.SetActive(true);
        }
        else
        {
            AbilityLock.SetActive(false);
        }

        
        
        Tooltip.SetTooltip(tooltipString,null,true);
    }

    protected virtual void Update()
    {   
        if(PressHold)
        {
            CORE.PlayerActor.ActorEntity.AttemptPrepareAbility(transform.GetSiblingIndex());
        }
        
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
                Animer.SetTrigger("Execute");
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
                Animer.SetTrigger("Cast");
                
            }

            CastingCooldownImage.fillAmount = CurrentAbility.CurrentCastingTime / CurrentAbility.CurrentAbility.CastingTime;
            CastingCooldownLabel.text = Mathf.RoundToInt(CurrentAbility.CurrentCastingTime).ToString();
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PressHold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PressHold = false;
    }
}
