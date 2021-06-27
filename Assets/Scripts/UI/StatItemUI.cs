using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatItemUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI StatLabel;

    public void SetStat(string attribute, string value)
    {
        StatLabel.text = "<color=white>"+attribute+ "</color>: "+ "<color=yellow>" + value+ "</color>";
    }

    public void SetStat(string attributeKey, string displayAttribute, float value)
    {
        if (ItemsLogic.DisplayAttributes.ContainsKey(attributeKey) && !string.IsNullOrEmpty(ItemsLogic.DisplayAttributes[attributeKey].SpriteName))
        {
            StatLabel.text = "<color="+(value > 0 ? ItemsLogic.GOOD_LINE_COLOR : ItemsLogic.BAD_LINE_COLOR )+"><sprite name=\"" + ItemsLogic.DisplayAttributes[attributeKey].SpriteName + "\">  "+ (value > 0 ? "+" : "") + Mathf.RoundToInt(value * 100f) + "% " + displayAttribute+"</color>";
        }
        else
        {
            StatLabel.text = "<color=" + (value > 0 ? ItemsLogic.GOOD_LINE_COLOR : ItemsLogic.BAD_LINE_COLOR) + ">" + (value > 0 ? "+" : "") + Mathf.RoundToInt(value * 100f) + "% " + displayAttribute + "</color>";
        }
    }
}
