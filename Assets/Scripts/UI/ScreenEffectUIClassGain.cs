using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUIClassGain : ScreenEffectUI
{
    [SerializeField]
    Image OrbIcon;

    [SerializeField]
    TextMeshProUGUI OrbNameLabel;


    public override void Show(object data)
    {
        base.Show(data);

        ClassJob classData = (ClassJob)data;

        this.OrbIcon.sprite = classData.Icon;
        this.OrbNameLabel.text = classData.DisplayName;
    }

}
