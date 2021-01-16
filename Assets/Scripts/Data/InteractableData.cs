using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Data/InteractableData", order = 2)]
[Serializable]
public class InteractableData : ScriptableObject
{
    public int Usages = 0;
    public int DurationInSeconds = 0;
    public string AbilityOnInteract;

    [JsonIgnore]
    public string InteractablePrefab;
}


