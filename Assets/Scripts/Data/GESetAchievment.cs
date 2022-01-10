using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GESetAchievment ", menuName = "Data/GESetAchievment ", order = 2)]
public class GESetAchievment : GameEvent
{
    public string AchievementKey;
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        #if !UNITY_ANDROID && !UNITY_IOS
        AchievementLogic.Instance.SetAchievment(AchievementKey);
        #endif
    }
}
