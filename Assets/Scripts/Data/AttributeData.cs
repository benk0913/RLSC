using System;
using Newtonsoft.Json;

//[CreateAssetMenu(fileName = "AttributeData", menuName = "Data/AttributeData", order = 2)]
[Serializable]
public class AttributeData// : ScriptableObject
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Power;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float HP;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Defense;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Block;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float CDReduction;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float CTReduction;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Lifesteal;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float LongRangeMultiplier;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float ShortRangeMultiplier;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float WildMagicChance;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float SpellDuration;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float AntiDebuff;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Threat;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float MovementSpeed;
    
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float DoubleCast;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float Explode;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float HpRegen;
}