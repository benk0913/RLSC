using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===================
// THIS IS DEPRECATED - use GameCondition
// ===================
[CreateAssetMenu(fileName = "OldCondition", menuName = "Data/Condition/OldCondition", order = 2)]
[Serializable]
[Obsolete("Use GameCondition instead")]
public class AbilityCondition : ScriptableObject, ConditionBase
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ConditionType Type;
    public string Value;
    [JsonConverter(typeof(ObjectNameConverter))]
    public ScriptableObject ObjectValue;
    public bool Inverse;

    public bool IsValid(System.Object obj)
    {
        return ConditionLogic.IsValid(obj, ObjectValue, Value, Type.ToString(), Inverse);
    }
}

[Serializable]
public enum ConditionType
{
    HasBuff,
    HasState,
    Staring,
    Chance,

    InExpeditionQueue,
    HasMoney,
    InExpedition,
    FinishedQuest,
    QuestStarted,
    CanFinishQuest,
    CanStartQuest

    // ===================
    // THIS IS DEPRECATED - use GameCondition
    // ===================
}