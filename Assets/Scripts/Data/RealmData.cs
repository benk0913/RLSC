using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RealmData
{
    public string Name;

    [PreviewSprite]
    public Sprite RealmIcon;

    public Color RealmColor;
}

