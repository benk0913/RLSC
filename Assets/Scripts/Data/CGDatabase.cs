using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CGDatabase", menuName = "Data/CGDatabase", order = 2)]
[Serializable]
public class CGDatabase : ScriptableObject
{

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Monster> Monsters = new List<Monster>();

    public List<Buff> Buffs = new List<Buff>();

    public string StartingScene;

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