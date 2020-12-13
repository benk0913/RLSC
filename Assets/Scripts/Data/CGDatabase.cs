using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CGDatabase", menuName = "Data/CGDatabase", order = 2)]
[Serializable]
public class CGDatabase : ScriptableObject
{
    [JsonIgnore]
    public string HostURL;

    public string unic0rn = "b0ss";

    public string DataPath = "Data";

    public DatabaseContent content;
}

[Serializable]
public class DatabaseContent
{

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Monster> Monsters = new List<Monster>();

    public List<Buff> Buffs = new List<Buff>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public string StartingScene;

    public string HumanPrefab;

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
    public float MovementSpeed = 12f;

}

[Serializable]
public class SceneInfo
{
    public string sceneName;
    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public float playerSpawnX;
    public float playerSpawnY;
}

[Serializable]
public class MobSpawn
{
    public string monsterName;
    public float positionX;
    public float positionY;
    
}
