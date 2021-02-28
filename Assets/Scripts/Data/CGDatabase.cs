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
    public bool IsLocal = false;
 
    [JsonIgnore]
    public string ProdHostURL;
 
    [JsonIgnore]
    public string LocalHostURL;

    [JsonIgnore]
    public string HostURL { get { return IsLocal ? LocalHostURL : ProdHostURL; }}

    public string unic0rn = "b0ss";

    public string DataPath = "Data";

    public DatabaseContent content;

    [JsonIgnore]
    public Material DefaultSpriteMaterial;
}

[Serializable]
public class DatabaseContent
{

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Monster> Monsters = new List<Monster>();

    public List<Buff> Buffs = new List<Buff>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public List<InteractableData> Interactables = new List<InteractableData>();

    public string StartingScene;

    public string HumanPrefab;

    public AttributeData BaseAttributes;
    
    public AttributeData AttributesLimits;
}

[Serializable]
public class SceneInfo
{
    public string sceneName;
    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public List<SceneInteractable> Interactables = new List<SceneInteractable>();
    public float playerSpawnX;
    public float playerSpawnY;

    [JsonIgnore]
    public string MusicTrack;
}

[Serializable]
public class SceneInteractable
{
    public string interactableName;
    public int positionX;
    public int positionY;
}

[Serializable]
public class MobSpawn
{
    public string monsterName;
    public float positionX;
    public float positionY;
    public float respawnSeconds;
}
