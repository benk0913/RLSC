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

    public string DisplayName;

    [TextArea(3,6)]
    public string Description;

    public string UniquePrefab;

    public AttributeData Stats;

    public int VendorPrice = 2;

    public int CashItemPrice = 0;

    //Between 0 to 1.
    public float DropChance = 1f;

    public ItemType Type;

    public ItemRarity Rarity;

    public bool CashShopItem = false;

    [JsonIgnore]
    public string OrbColliderObject;

    [JsonIgnore]
    public List<SkinSet> SkinOverride = new List<SkinSet>();
    
    public List<State> States = new List<State>();
    
    [JsonIgnore]
    public List<TypeBasedOverride> TypeBasedOverrides = new List<TypeBasedOverride>();

    public List<ItemData> Pool = new List<ItemData>();

    public List<AbilityParam> OnExecuteParams = new List<AbilityParam>();
    
    public List<AbilityParam> OnHitParams = new List<AbilityParam>();

    public SkinSet SkinTypeOverride(string typeKey)
    {
        TypeBasedOverride result = TypeBasedOverrides.Find(x => x.TypeKey == typeKey);

        if (result != null)
        {
            return result.Skinset;
        }

        return null;
    }

    [Serializable]
    public class TypeBasedOverride
    {
        public string TypeKey;

        public SkinSet Skinset;
    }
}

