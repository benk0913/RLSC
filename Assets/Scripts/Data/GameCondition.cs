using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GameCondition: ConditionBase
{
    public GameConditionType Type;

    public string Value;

    [JsonConverter(typeof(ObjectNameConverter))]
    public ScriptableObject ObjectValue;

    public bool Inverse;

    public bool IsValid(System.Object obj)
    {
        return ConditionLogic.IsValid(obj, ObjectValue, Value, Type.name, Inverse);
    }
}