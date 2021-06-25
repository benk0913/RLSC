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
}

[Serializable]
public class ExpeditionFloor
{
    public List<string> PossibleChambers = new List<string>();
    public int Exp;
}