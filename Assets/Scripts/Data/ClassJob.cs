using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassJob", menuName = "Data/ClassJob", order = 2)]
[Serializable]
public class ClassJob : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    public List<string> Abilities = new List<string>();
    
    public string PassiveAbility;

    public AttributeData Attributes;
    
    public List<State> Immunity;
    
    public List<State> States;

    [JsonIgnore]
    public Color ClassColor;

    public List<ItemData> DropsOnDeath = new List<ItemData>();
    
    public float CoinDropMultiplier = 0f;
    
    public List<AbilityParam> OnDeathParams = new List<AbilityParam>();

    [JsonIgnore]
    public List<string> UniqueHurtSounds = new List<string>();

    [JsonIgnore]
    public string UniqueDeathSound;
}