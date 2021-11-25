using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Expedition", menuName = "Data/Expedition", order = 2)]
[Serializable]
public class Expedition : ScriptableObject
{
    public List<ExpeditionFloor> Floors = new List<ExpeditionFloor>();
    public int PartyLengthMin;
    public int PartyLengthMax;
    public List<ExpeditionQueueRule> QueueRules = new List<ExpeditionQueueRule>();

    public string EndTargetScene;
    public string EndTargetSceneGate;
    public string SuccessTargetScene;
    public string SuccessTargetSceneGate;

    public bool ContainsScene(string sceneName)
    {
        foreach (ExpeditionFloor floor in Floors)
        {
            foreach(string sceneChamber in floor.PossibleChambers)
            {
                if(sceneChamber == sceneName)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[Serializable]
public class ExpeditionFloor
{
    public List<string> PossibleChambers = new List<string>();
    public int Exp;
    public int FloorPrediction;
}

[Serializable]
public class ExpeditionQueueRule
{
    public bool RequireAllClasses;
    public bool RequireAirOrEarth;
    public int PartyLength;
    public int SecondsToStartRule;
}