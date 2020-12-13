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
    public string BuffColliderObject;

    public float Length = 1f;
    public bool isDebuff = false;

    [JsonIgnore]
    public bool ShowBuffIcon = true;
    
    public AttributeData Attributes;

    public bool MakesInvulnerable = false;

    public List<AbilityParam> OnStart = new List<AbilityParam>();
    public List<AbilityParam> OnEnd = new List<AbilityParam>();
}
