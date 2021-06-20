using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectUILocation : ScreenEffectUI
{
    [SerializeField]
    TextMeshProUGUI LocationLabel;

    public override void Show(object data)
    {
        base.Show(data);

        LocationLabel.text = (string)data;
    }

}
