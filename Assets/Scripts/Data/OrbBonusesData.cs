using System;
using Newtonsoft.Json;

[Serializable]
public class OrbBonusesData
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float DoubleCast;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int ExtraLife;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float StunOnDmg;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int LockSlot;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float SpawnSlime;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Wings;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool InvulnerableOnIdle;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float HpRegen;
}