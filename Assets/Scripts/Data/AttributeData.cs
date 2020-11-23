using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "AttributeData", menuName = "Data/AttributeData", order = 2)]
[Serializable]
public class AttributeData// : ScriptableObject
{
    public float Power;
    public float HP;
    public float Defense;
    public float CDReduction;
    public float Lifesteal;
    public float CriticalChance;
    public float LongRangeMultiplier;
    public float ShortRangeMultiplier;
    public float WildMagicChance;
    public float SingleTargetDamage;
    public float AOEDamage;
    public float SpellDuration;
    public float AntiDebuff;
    public float KnockbackResistance;
    public float SmallerColliderSize;
    public float Threat;
}