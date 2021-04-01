using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Items/ItemData", order = 2)]
[Serializable]
public class ItemData : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;

    [TextArea(3,6)]
    public string Description;

    public AttributeData Stats;

    //Between 0 to 1.
    public float DropChance = 1f;

    public ItemType Type;

    public ItemRarity Rarity;

    [JsonIgnore]
    public List<SkinSet> SkinOverride = new List<SkinSet>();
    
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public OrbBonusesData OrbBonuses;
}

