using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlotUI : MonoBehaviour
{
    BuffSlotEffect ActiveBuffEffect;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Image CooldownImage;

    [SerializeField]
    TextMeshProUGUI CooldownLabel;

    [SerializeField]
    TooltipTargetUI Tooltip;
 
    public void SetBuffState(BuffState buffState)
    {
        ActiveBuffEffect = new BuffSlotEffect(buffState);
        UpdateBuffEffect();
    }
 
    public void SetOrb(Item orb)
    {
        ActiveBuffEffect = new BuffSlotEffect(orb);
        UpdateBuffEffect();
    }

    private void UpdateBuffEffect()
    {
        IconImage.sprite = ActiveBuffEffect.Icon;

        string tooltipString = "";


        string buffName = ActiveBuffEffect.Name;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(ActiveBuffEffect.Name, out buffName);

        string buffDesc = ActiveBuffEffect.Description;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslationCodywise(ActiveBuffEffect.Description, out buffDesc);

        tooltipString += "<color=" + Colors.COLOR_HIGHLIGHT + ">"+buffName+"</color>";
        tooltipString += buffDesc;
        Tooltip.SetTooltip(tooltipString,null,true);
    }

    private void Update()
    {
        if(ActiveBuffEffect == null)
        {
            return;
        }

        if(ActiveBuffEffect.CurrentLength <= 0 && CooldownImage.gameObject.activeInHierarchy)
        {
            CooldownImage.gameObject.SetActive(false);
        }
        else if(ActiveBuffEffect.CurrentLength > 0)
        {
            if (!CooldownImage.gameObject.activeInHierarchy)
            {
                CooldownImage.gameObject.SetActive(true);
            }

            CooldownImage.fillAmount = ActiveBuffEffect.CurrentLength / ActiveBuffEffect.Length;
            CooldownImage.color = ActiveBuffEffect.IsNegativeEffect ? Colors.AsColor(Colors.COLOR_BAD) : Colors.AsColor(Colors.COLOR_GOOD);

            CooldownLabel.text = Mathf.RoundToInt(ActiveBuffEffect.CurrentLength).ToString();
            CooldownLabel.color = CooldownImage.color;
        }
    }
}

public class BuffSlotEffect
{
    public BuffState BuffState;
    public Sprite Icon;
    public string Name;
    public string Description;
    public float Length
    {
        get
        {
            if (BuffState == null)
            {
                return 0;
            }
            return BuffState.Length;
        }
    }
    public float CurrentLength
    {
        get
        {
            if (BuffState == null)
            {
                return 0;
            }
            return BuffState.CurrentLength;
        }
    }
    public bool IsNegativeEffect;

    public BuffSlotEffect(BuffState buffState)
    {
        BuffState = buffState;
        Icon = buffState.CurrentBuff.Icon;
        Name = buffState.CurrentBuff.name;
        Description = System.Environment.NewLine + buffState.CurrentBuff.Description;
        IsNegativeEffect = buffState.CurrentBuff.isDebuff;
    }

    public BuffSlotEffect(Item orb)
    {
        BuffState = null;
        Icon = orb.Data.Icon;
        Name = orb.Data.DisplayName;
        Description = ItemsLogic.GetTooltipTextFromItem(orb.Data);
    }
}