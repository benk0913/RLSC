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

        tooltipString += "<color=yellow>"+ActiveBuffEffect.Name+"</color>";
        tooltipString += System.Environment.NewLine + ActiveBuffEffect.Description;
        Tooltip.SetTooltip(tooltipString);
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
            CooldownImage.color = ActiveBuffEffect.IsNegativeEffect ? Color.red : Color.green;

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
        Description = buffState.CurrentBuff.Description;
        IsNegativeEffect = buffState.CurrentBuff.isDebuff;
    }

    public BuffSlotEffect(Item orb)
    {
        BuffState = null;
        Icon = orb.Data.Icon;
        Name = orb.Data.name;
        Description = orb.Data.Description;
    }
}