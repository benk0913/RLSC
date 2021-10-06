using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemsLogic
{
    public static string GOOD_LINE_COLOR = "#8AFD97";
    public static string BAD_LINE_COLOR = "#F28B7D";

    public static Dictionary<string, DisplayAttribute> DisplayAttributes = new Dictionary<string, DisplayAttribute>()
    {
        { "Power", new DisplayAttribute(typeof(AttributeData).GetField("Power"), "icat_9", "Damage","")},
        { "HP", new DisplayAttribute(typeof(AttributeData).GetField("HP"), "icat_0", "HP","")},
        { "Defense", new DisplayAttribute(typeof(AttributeData).GetField("Defense"), "icat_10", "Defense","")},
        { "Block", new DisplayAttribute(typeof(AttributeData).GetField("Block"), "icat_10", "Block","")},
        { "CDReduction", new DisplayAttribute(typeof(AttributeData).GetField("CDReduction"), "icat_8", "Lower Cooldowns","")},
        { "CTReduction", new DisplayAttribute(typeof(AttributeData).GetField("CTReduction"), "icat_7", "Faster Spells","")},
        { "Lifesteal", new DisplayAttribute(typeof(AttributeData).GetField("Lifesteal"), "lifesteal", "Lifesteal","")},
        { "LongRangeMultiplier", new DisplayAttribute(typeof(AttributeData).GetField("LongRangeMultiplier"), "crosshair", "Distance Damage","")},
        { "ShortRangeMultiplier", new DisplayAttribute(typeof(AttributeData).GetField("ShortRangeMultiplier"), "icat_6", "Close Damage","")},
        { "WildMagicChance", new DisplayAttribute(typeof(AttributeData).GetField("WildMagicChance"), "icat_5", "Wild Magic","")},
        { "SpellDuration", new DisplayAttribute(typeof(AttributeData).GetField("SpellDuration"), "icat_4", "Buffs Duration","")},
        { "AntiDebuff", new DisplayAttribute(typeof(AttributeData).GetField("AntiDebuff"), "icat_3", "Debuffs Resistance","")},
        { "Threat", new DisplayAttribute(typeof(AttributeData).GetField("Threat"), "icat_2", "Threat","")},
        { "MovementSpeed", new DisplayAttribute(typeof(AttributeData).GetField("MovementSpeed"), "icat_1", "Speed","")},
        { "JumpHeight", new DisplayAttribute(typeof(AttributeData).GetField("JumpHeight"), "icat_1", "Jump","")},
        { "DoubleCast", new DisplayAttribute(typeof(AttributeData).GetField("DoubleCast"), "Default", "Double Cast","")},
        { "Explode", new DisplayAttribute(typeof(AttributeData).GetField("Explode"), "icat_9", "Explode","")},
        { "HpRegen", new DisplayAttribute(typeof(AttributeData).GetField("HpRegen"), "hpregen", "HP Regen","")},
        { "Default", new DisplayAttribute(null, "Default", "Default","")},
    };
    public static string GetTooltipTextFromItem(ItemData itemData)
    {
        string text = "";
        text += GetTooltipTextFromAttributes(itemData.Stats);
        text += GetTooltipTextFromAbilityParams(itemData.OnExecuteParams, " on execute");
        text += GetTooltipTextFromAbilityParams(itemData.OnHitParams, " on hit");
        return text;
    }

    public static string GetTooltipTextFromAttributes(AttributeData data)
    {
        string result = "";

        // First get all the positives, then the negatives.
        foreach (KeyValuePair<string, DisplayAttribute> keyValuePair in DisplayAttributes)
        {
            float propertyValue = keyValuePair.Value.FieldInfo != null ? (float)keyValuePair.Value.FieldInfo.GetValue(data) : 0f;
            
            if (propertyValue > 0)
            {
                string icon = string.IsNullOrEmpty(keyValuePair.Value.SpriteName) ?  "<sprite name=\"Default\">" : "<sprite name=\"" + keyValuePair.Value.SpriteName + "\">  ";
                result += Environment.NewLine + "<color=" + GOOD_LINE_COLOR + ">" + icon + keyValuePair.Value.Name + " +" + Mathf.RoundToInt(propertyValue * 100)+"%" + "</color>";
            }
        }
        foreach (KeyValuePair<string, DisplayAttribute> keyValuePair in DisplayAttributes)
        {
            float propertyValue = keyValuePair.Value.FieldInfo != null ? (float)keyValuePair.Value.FieldInfo.GetValue(data) : 0f;

            if (propertyValue < 0)
            {
                string icon = string.IsNullOrEmpty(keyValuePair.Value.SpriteName) ? "<sprite name=\"Default\">" : "<sprite name=\"" + keyValuePair.Value.SpriteName + "\" tint=1>  ";
                result += Environment.NewLine + "<color=" + BAD_LINE_COLOR + ">" + icon + keyValuePair.Value.Name + " " + Mathf.RoundToInt( propertyValue * 100)+"%" + "</color>";
            }
        }

        return result;
    }

    private static string GetTooltipTextFromAbilityParams(List<AbilityParam> abilityParams, string whenCondition)
    {
        string text = "";
        foreach (var abilityParam in abilityParams)
        {
            string abilityParamText = "";

            abilityParamText += "<sprite name=\"crosshair\">  ";

            if (abilityParam.Condition && abilityParam.Condition.Type == ConditionType.Chance)
            {
                abilityParamText += Mathf.CeilToInt(float.Parse(abilityParam.Condition.Value) * 100) + "% chance to ";
            }

            abilityParamText += abilityParam.Type.name + " on " + abilityParam.Targets.ToString() + whenCondition;
            string value = abilityParam.ObjectValue == null ? abilityParam.Value : abilityParam.ObjectValue.name;
            if (!string.IsNullOrEmpty(value))
            {
                abilityParamText += ": " + value;
            }
            abilityParamText = char.ToUpper(abilityParamText[0]) + abilityParamText.Substring(1).ToLower();

            text += Environment.NewLine + "<color=" + GOOD_LINE_COLOR + ">" +  abilityParamText + "</color>";
        }
        return text;
    }

    public static string GetItemTooltip(ItemData itemData)
    {
        string text = itemData.DisplayName;
        text += System.Environment.NewLine +"<i><color=yellow>"+ itemData.Type.name+"</color></i>";



        string description = itemData.Description.Trim();
        if (!string.IsNullOrEmpty(description)) {
            text += System.Environment.NewLine + "<i>"+description+"</i>";
        }

        //text += System.Environment.NewLine + "<i><u><color=yellow> Bonuses: </color></u></i>";
        text += ItemsLogic.GetTooltipTextFromItem(itemData);

        return text;
    }

    public class DisplayAttribute
    {
        public FieldInfo FieldInfo;
        public string SpriteName;
        public string Name;
        public string Description;

        public DisplayAttribute(FieldInfo fieldInfo, string spriteName, string name, string description) {
            this.FieldInfo = fieldInfo;
            this.SpriteName = spriteName;
            this.Name = name;
            this.Description = description;
        }
    }
}