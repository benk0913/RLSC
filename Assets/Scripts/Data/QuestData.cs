using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Data/Quests/QuestData", order = 2)]
[Serializable]
public class QuestData: ScriptableObject
{
    public List<AbilityCondition> StartConditions;

    public List<QuestGoal> Goals;

    public List<AbilityParam> Rewards;
}
