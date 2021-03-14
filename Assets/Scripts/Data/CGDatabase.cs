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
    public VisualDatabase Visuals;

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Buff> Buffs = new List<Buff>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public List<InteractableData> Interactables = new List<InteractableData>();
    
    public List<Expedition> Expeditions = new List<Expedition>();
    
    public List<int> ExpChart = new List<int>();

    public string StartingScene;

    public string HumanPrefab;

    public AttributeData BaseAttributes;
    
    public AttributeData AttributesLimits;

    public int MaxLevel;

    public int AbilitiesInitCount;

    public int AbilitiesMaxCount;
}

[Serializable]
public class VisualDatabase
{
    [JsonIgnore]
    public List<BodyPart> BodyParts = new List<BodyPart>();

    [JsonIgnore]
    public List<SkinSet> DefaultSkin = new List<SkinSet>();

    [JsonIgnore]
    public List<SkinSet> SkinSets = new List<SkinSet>();

    [JsonIgnore]
    public List<Color> SkinColorPresets = new List<Color>();
    [JsonIgnore]
    public List<Color> HairColorPresets = new List<Color>();
    [JsonIgnore]
    public List<Color> IrisColorPresets = new List<Color>();
}

[Serializable]
public class SceneInfo
{
    public string sceneName;
    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public List<SceneInteractable> Interactables = new List<SceneInteractable>();
    public float playerSpawnX;
    public float playerSpawnY;
    public List<Portal> Portals = new List<Portal>();

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
