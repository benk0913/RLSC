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
    public AbilityColliders Colliders;

    [JsonIgnore]
    public AbilitySounds Sounds;

    [JsonIgnore]
    public AbilityVisuals Visuals;

    public float CD = 1f;

    [JsonIgnore]
    public float CastingTime = 0.003f;//0.003f is the min (Lag Compensation).

    [JsonIgnore]
    public int TargetCap;

    [JsonIgnore]
    public bool OnlyIfGrounded;

    [JsonIgnore]
    public float AIViableRange = 0f;

    [JsonIgnore]
    public bool IsCastingExternal;


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

    [JsonConverter(typeof(ObjectNameConverter))]
    public ScriptableObject ObjectValue;

    public string Value2;
}

[Serializable]
public class AbilityColliders
{
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


}

[Serializable]
public class AbilitySounds
{
    [JsonIgnore]
    public string PrepareAbilitySound;

    [JsonIgnore]
    public string ExecuteAbilitySound;

    [JsonIgnore]
    public string HitAbilitySound;

    [JsonIgnore]
    public List<string> HitAbilitySoundVarriants = new List<string>();

    [JsonIgnore]
    public string MissAbilitySound;


}

[Serializable]
public class AbilityVisuals
{
    [JsonIgnore]
    public string ScreenEffectObject;

    [JsonIgnore]
    public string PreparingAnimation;

    [JsonIgnore]
    public string ExecuteAnimation;
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
