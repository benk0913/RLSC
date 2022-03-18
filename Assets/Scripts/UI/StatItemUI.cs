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
        StatLabel.text = "<color=" + Colors.COLOR_TEXT+ ">"+ CORE.QuickTranslate(attribute) + "</color>: "+ "<color=" + Colors.COLOR_HIGHLIGHT + ">" + value+ "</color>";
    }

    public void SetStat(string attributeKey, string displayAttribute, float value)
    {
        if (Util.DisplayAttributes.ContainsKey(attributeKey) && !string.IsNullOrEmpty(Util.DisplayAttributes[attributeKey].SpriteName))
        {
            StatLabel.text = "<color="+(value > 0 ? Colors.COLOR_GOOD : Colors.COLOR_BAD )+"><sprite name=\"" + Util.DisplayAttributes[attributeKey].SpriteName + "\">  "+ (value > 0 ? "+" : "") + Mathf.RoundToInt(value * 100f) + "% " +CORE.QuickTranslate(displayAttribute)+"</color>";
        }
        else
        {
            StatLabel.text = "<color=" + (value > 0 ? Colors.COLOR_GOOD : Colors.COLOR_BAD) + ">" + (value > 0 ? "+" : "") + Mathf.RoundToInt(value * 100f) + "% " + CORE.QuickTranslate(displayAttribute) + "</color>";
        }
    }
}
