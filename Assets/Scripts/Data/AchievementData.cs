using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "Data/Achievements/AchievementData", order = 2)]
[Serializable]
public class AchievementData: ScriptableObject
{
    public string Key;
    public AchievementAction Action;
    [JsonConverter(typeof(ObjectNameConverter))]
    public List<ScriptableObject> ObjectValues;
    public List<string> Values;
    public List<GameCondition> GameConditions;
    public int Count = 1;
}