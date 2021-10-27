using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUISpellGain : ScreenEffectUI
{
    [SerializeField]
    Image OrbIcon;

    [SerializeField]
    TextMeshProUGUI OrbNameLabel;

    [SerializeField]
    TextMeshProUGUI KeyLabel;


    public override void Show(object data)
    {
        base.Show(data);

        Ability abilityData = (Ability)data;

        this.OrbIcon.sprite = abilityData.Icon;
        this.OrbNameLabel.text = abilityData.name;

        this.KeyLabel.text = CORE.QuickTranslate("Press '"+InputMap.Map["Abilities Window"].ToString()+"' to View");
    }

}
