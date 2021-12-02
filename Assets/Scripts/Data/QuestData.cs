using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Data/Quests/QuestData", order = 2)]
[Serializable]
public class QuestData: ScriptableObject
{
    public List<AbilityCondition> StartConditions;
    
    public List<GameCondition> StartGameConditions;

    public List<QuestGoal> Goals;

    public List<AbilityParam> Rewards;

    [JsonIgnore]
    public bool CanStart
    {
        get
        {
            foreach(AbilityCondition condition in StartConditions)
            {
                if(!condition.IsValid(null))
                {
                    return false;
                }
            }

            foreach (GameCondition condition in StartGameConditions)
            {
                if (!condition.IsValid(null))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
