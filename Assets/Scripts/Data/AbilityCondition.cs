using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition", menuName = "Data/Condition", order = 2)]
[Serializable]
public class AbilityCondition : ScriptableObject //TODO RENAME TO GameCondition
{
    [JsonConverter(typeof(StringEnumConverter))]
    public ConditionType Type;
    public string Value;
    public bool Inverse;

    public bool IsValid(System.Object obj)
    {
        switch(Type)
        {
            case ConditionType.HasBuff:
                {
                    Actor target = ((Actor)obj);

                    if(target.State.Buffs.Find(x=>x.CurrentBuff.name == Value) != null)
                    {
                        return !Inverse; //  False
                    }
                    else
                    {
                        return Inverse; //  True
                    }
                }
            case ConditionType.Chance:
                {
                    if(UnityEngine.Random.Range(0f,1f) < float.Parse(Value))
                    {
                        return !Inverse; //  False
                    }
                    else
                    {
                        return Inverse; //  True
                    }
                }
            case ConditionType.InExpeditionQueue:
                {
                    if(ExpeditionQueTimerUI.Instance.IsSearching)
                    {
                        return !Inverse;
                    }
                    else
                    {
                        return Inverse;
                    }
                }
        } 

        return !Inverse; //  True 
    }
}

[Serializable]
public enum ConditionType
{
    HasBuff,
    HasState,
    Staring,
    Chance,

    InExpeditionQueue
}