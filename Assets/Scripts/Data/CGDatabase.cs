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

    public string StartingScene;

    public string HumanPrefab;

    public AttributeData BaseAttributes;
    
    public AttributeData AttributesLimits;
}

[Serializable]
public class VisualDatabase
{
    public List<BodyPart> BodyParts = new List<BodyPart>();

    public List<SkinSet> DefaultSkin = new List<SkinSet>();

    public List<SkinSet> SkinSets = new List<SkinSet>();

    public List<Color> SkinColorPresets = new List<Color>();
    public List<Color> HairColorPresets = new List<Color>();
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
