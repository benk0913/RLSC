using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string itemId;
    
    public float x;
    public float y;

    [SerializeField]
    protected string data;

    [JsonIgnore]
    public ItemEntity Entity;

    [JsonIgnore]
    public ItemData Data
    {
        get
        {
            if(_itemData == null)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }

                _itemData = CORE.Instance.Data.content.Items.Find(X => X.name == data);
            }

            return _itemData;
        }
    }

    [JsonIgnore]
    ItemData _itemData;

    public Item(ItemData itemDat)
    {
        this.data = itemDat.name;
    }

    public Item(string itemDataKey)
    {
        this.data = itemDataKey;
    }
}

