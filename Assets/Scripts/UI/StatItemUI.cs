using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatItemUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI StatLabel;

    public void SetStat(string attribute, float value)
    {
        if (ItemsLogic.DisplayAttributes.ContainsKey(attribute) && !string.IsNullOrEmpty(ItemsLogic.DisplayAttributes[attribute].SpriteName))
        {
            StatLabel.text = "<sprite name=\"" + ItemsLogic.DisplayAttributes[attribute].SpriteName + "\"> " + attribute + ": " + value;
        }
        else
        {
            StatLabel.text = attribute + ": " + value;
        }
    }
}
