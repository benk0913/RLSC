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

    public override void Show(object data)
    {
        base.Show(data);
        ItemData itemData = (ItemData)data;

        this.OrbIcon.sprite = itemData.Icon;
        this.OrbNameLabel.text = itemData.name;
    }

}
