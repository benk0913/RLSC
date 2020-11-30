using Newtonsoft.Json;
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
    public string AbilityColliderObject;

    public float CD = 1f;
    public float CastingTime = 0.003f;//0.003f is the min (Lag Compensation).

    [JsonIgnore]
    public string PreparingAnimation;

    [JsonIgnore]
    public string ExecuteAnimation;

    public AttributeData Attributes;

    public List<AbilityParam> OnExecuteParams = new List<AbilityParam>();
    public List<AbilityParam> OnHitParams = new List<AbilityParam>();
}

[Serializable]
public class AbilityParam
{
    public AbilityParamType Type;
    public TargetType Targets;
    public string Value;
}

[Serializable]
public enum TargetType
{
    Enemies,
    Friends,
    Self
}
