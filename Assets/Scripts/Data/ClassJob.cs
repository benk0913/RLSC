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

    public AttributeData Attributes;
}