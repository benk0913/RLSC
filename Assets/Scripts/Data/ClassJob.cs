using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "ClassJob", menuName = "Data/ClassJob", order = 2)]
[Serializable]
public class ClassJob : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    public string DisplayName;

    public List<string> Abilities = new List<string>();
    
    public string PassiveAbility;

    public AttributeData Attributes;
    
    public List<State> Immunity;
    
    public List<State> States;

    [JsonIgnore]
    public Color ClassColor;

    [JsonIgnore]
    public VideoClip ClassFeatureVideo;

    [JsonIgnore]
    public AudioClip ClassMusic;

    public List<ItemData> DropsOnDeath = new List<ItemData>();
    
    public float CoinDropMultiplier = 0f;

    public float EXPMultiplier = 0f;
    
    public List<AbilityParam> OnGetHitParams = new List<AbilityParam>();
    
    public List<AbilityParam> OnSomeoneExecutesParams = new List<AbilityParam>();

    public List<AbilityParam> OnDeathParams = new List<AbilityParam>();

    [JsonIgnore]
    public List<string> UniqueHurtSounds = new List<string>();

    [JsonIgnore]
    public List<string> UniqueBlockSounds = new List<string>();
    
    [JsonIgnore]
    public string UniqueDeathSound;
}