using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Emote", menuName = "Data/Emote", order = 2)]
[Serializable]
public class Emote : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite EmoteGraphic;

}

