using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Data/Buff", order = 2)]
[Serializable]
public class Buff : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;
    
    [JsonIgnore]
    [TextArea(3, 6)]
    public string Description;

    [JsonIgnore]
    public string BuffColliderObject;

    [JsonIgnore]
    public Material BuffMaterial;

    public float Length = 1f;
    public bool isDebuff = false;

    [JsonIgnore]
    public bool ShowBuffIcon = true;
    
    public AttributeData Attributes;

    public Ability HitAbility;

    [JsonIgnore]
    public string OnStartSound;

    [JsonIgnore]
    public string OnEndSound;

    [JsonIgnore]
    public bool DontReplaySoundOnRecharge = false;

    public List<State> States = new List<State>();

    public List<AbilityParam> OnStart = new List<AbilityParam>();
    public List<AbilityParam> OnEnd = new List<AbilityParam>();
    public List<AbilityParam> OnTakeDamage = new List<AbilityParam>();


}
public class BuffLogic
{
    public static string GetTooltipTextFromBuff(Buff buffData)
    {
        string text = "";
        text += Util.GetTooltipTextFromAttributes(buffData.Attributes);
        text +=  Util.GetTooltipTextFromAbilityParams(buffData.OnStart, "on start");
        text +=  Util.GetTooltipTextFromAbilityParams(buffData.OnEnd, "on end");
        text +=  Util.GetTooltipTextFromAbilityParams(buffData.OnTakeDamage, "on take damage");
        return text;
    }

    public static string GetBuffTooltip(Buff buffData)
    {
        string text = CORE.QuickTranslate(buffData.name);
        
        string description = CORE.QuickTranslate(buffData.Description).Trim();

        if (!string.IsNullOrEmpty(description)) {
            text += System.Environment.NewLine + "<i>"+description+"</i>";
        }

        //text += System.Environment.NewLine + "<i><u><color=" + Colors.COLOR_HIGHLIGHT + "> Bonuses: </color></u></i>";
        text += GetTooltipTextFromBuff(buffData);

        return text;
    }


}