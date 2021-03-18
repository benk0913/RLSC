using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemType", menuName = "Data/Items/ItemType", order = 2)]
[Serializable]
public class ItemType : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite Icon;
}

