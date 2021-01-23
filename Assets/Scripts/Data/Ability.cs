using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Data/Ability", order = 2)]
[Serializable]
public class Ability : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    [JsonIgnore]
    [TextArea(3, 6)]
    public string Description;

    [JsonIgnore]
    public string HitAbilityColliderObject;

    [JsonIgnore]
    public string MissAbilityColliderObject;

    [JsonIgnore]
    public string AbilityColliderObject;

    [JsonIgnore]
    public string PrepareAbilityColliderObject;
    
    [JsonIgnore]
    public string HitConditionObject;

    [JsonIgnore]
    public AbilityCondition HitConditionObjectCondition;

    [JsonIgnore]
    public string PrepareAbilitySound;

    [JsonIgnore]
    public string ExecuteAbilitySound;

    [JsonIgnore]
    public string HitAbilitySound;
    public string MissAbilitySound;

    public float CD = 1f;
    public float CastingTime = 0.003f;//0.003f is the min (Lag Compensation).

    public bool OnlyIfGrounded;

    [JsonIgnore]
    public string PreparingAnimation;

    [JsonIgnore]
    public string ExecuteAnimation;

    public List<AbilityParam> OnExecuteParams = new List<AbilityParam>();
    public List<AbilityParam> OnHitParams = new List<AbilityParam>();
    public List<AbilityParam> OnMissParams = new List<AbilityParam>();
}

[Serializable]
public class AbilityParam
{
    public AbilityParamType Type;
    
    public AbilityCondition Condition;

    [JsonConverter(typeof(StringEnumConverter))]
    public TargetType Targets;

    public string Value;
}


[Serializable]
public enum TargetType
{
    Enemies,
    Friends,
    Self,
    NotSelf,
    FriendsAndSelf,
    All
}



