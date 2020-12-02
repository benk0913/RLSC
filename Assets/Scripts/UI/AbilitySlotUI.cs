using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    AbilityState CurrentAbility;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    Image CooldownImage;

    [SerializeField]
    TextMeshProUGUI CooldownLabel;

    [SerializeField]
    Image CastingCooldownImage;

    [SerializeField]
    TextMeshProUGUI CastingCooldownLabel;


    public void SetAbilityState(AbilityState abilityState = null)
    {
        CurrentAbility = abilityState;

        if(CurrentAbility == null)
        {
            IconImage.sprite = ResourcesLoader.Instance.GetSprite("emptySlot");
            return;
        }

        IconImage.sprite = CurrentAbility.CurrentAbility.Icon;
    }

    private void Update()
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
