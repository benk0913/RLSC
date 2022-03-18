using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Util;

public class ItemsLogic
{
    public static string GetTooltipTextFromItem(ItemData itemData)
    {
        string text = "";
        text += GetTooltipTextFromAttributes(itemData.Stats);
        text += GetTooltipTextFromAbilityParams(itemData.OnExecuteParams, "on execute");
        text += GetTooltipTextFromAbilityParams(itemData.OnHitParams, "on hit");
        return text;
    }

    public static string GetItemTooltip(ItemData itemData)
    {
        string text = CORE.QuickTranslate(itemData.DisplayName);
        
        text += System.Environment.NewLine +"<i><color=#" + ColorUtility.ToHtmlStringRGB(itemData.Rarity.RarityColor)+ ">"+ CORE.SplitCamelCase(CORE.QuickTranslate(itemData.Rarity.name))+"</color></i>";
        text += System.Environment.NewLine +"<i><color=" + Colors.COLOR_HIGHLIGHT + ">"+ CORE.SplitCamelCase(CORE.QuickTranslate(itemData.Type.name))+"</color></i>";
        
        string description = CORE.QuickTranslate(itemData.Description).Trim();

        if (!string.IsNullOrEmpty(description)) {
            text += System.Environment.NewLine + "<i>"+description+"</i>";
        }

        //text += System.Environment.NewLine + "<i><u><color=" + Colors.COLOR_HIGHLIGHT + "> Bonuses: </color></u></i>";
        text += GetTooltipTextFromItem(itemData);

        return text;
    }


}