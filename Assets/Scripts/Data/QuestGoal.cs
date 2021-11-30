using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestGoal
{
    public QuestAction Action;

    [JsonConverter(typeof(ObjectNameConverter))]
    public ScriptableObject ObjectValue;

    public string Value;
    
    public int Count = 1;
}