using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Data/GameEvent", order = 2)]
[Serializable]
public class GameEvent : ScriptableObject
{
    public virtual void Execute(System.Object obj = null)
    {
    }
}