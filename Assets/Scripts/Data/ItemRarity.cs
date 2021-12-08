using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRarity", menuName = "Data/Items/ItemRarity", order = 2)]
[Serializable]
public class ItemRarity : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    [JsonIgnore]
    public Color RarityColor;

}

