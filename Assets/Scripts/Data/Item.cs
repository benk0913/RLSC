using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string itemId;
    public string itemName;
    public float x;
    public float y;
    public float spawnX;
    public float spawnY;
    public int amount;

    [JsonIgnore]
    public ItemEntity Entity;

    [JsonIgnore]
    public ItemData Data
    {
        get
        {
            if(_itemData == null)
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                _itemData = CORE.Instance.Data.content.Items.Find(X => X.name == itemName);
            }

            return _itemData;
        }
        set
        {
            _itemData = value;
        }
    }

    [JsonIgnore]
    ItemData _itemData;

    [JsonIgnore]
    public Material OrbMaterial;

}

