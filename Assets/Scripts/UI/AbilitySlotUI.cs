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


    public virtual void SetAbilityState(AbilityState abilityState = null)
    {

        CurrentAbility = abilityState;

        if(CurrentAbility == null || CurrentAbility.CurrentAbility == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = CurrentAbility.CurrentAbility.Icon;
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
