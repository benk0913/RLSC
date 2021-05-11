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

    public List<Item> Pool = new List<Item>();

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
    }

    [JsonIgnore]
    ItemData _itemData;

}

