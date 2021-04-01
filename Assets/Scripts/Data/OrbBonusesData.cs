using System;
using Newtonsoft.Json;

[Serializable]
public class OrbBonusesData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float DoubleCast;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float StunOnDmg;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float SpawnSlime;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float HpRegen;
}