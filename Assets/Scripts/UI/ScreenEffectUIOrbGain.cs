using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUIOrbGain : ScreenEffectUI
{
    [SerializeField]
    Image OrbIcon;

    [SerializeField]
    TextMeshProUGUI OrbNameLabel;



    [SerializeField]
    TextMeshProUGUI TriggerText;

    public override void Show(object data)
    {
        base.Show(data);
        ItemData itemData = (ItemData)data;

        this.OrbIcon.sprite = itemData.Icon;
        this.OrbNameLabel.text = itemData.DisplayName;

        if(itemData.Icon.name.Contains("curse"))
        {
            this.TriggerText.text = CORE.QuickTranslate("Cursed!");
        }
        else
        {
            this.TriggerText.text = CORE.QuickTranslate("Orb In Effect!");
        }
    }

}
