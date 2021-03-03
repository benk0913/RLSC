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

    public List<State> States = new List<State>();

    public List<AbilityParam> OnStart = new List<AbilityParam>();
    public List<AbilityParam> OnEnd = new List<AbilityParam>();
    public List<AbilityParam> OnTakeDamage = new List<AbilityParam>();
}
