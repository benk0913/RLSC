using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnDemandParams", menuName = "Data/OnDemandParams", order = 2)]
[Serializable]
public class OnDemandParams : ScriptableObject
{
    public List<AbilityParam> AbilityParams = new List<AbilityParam>();
}
