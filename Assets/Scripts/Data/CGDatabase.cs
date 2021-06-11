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

    [JsonIgnore]
    public Sprite ErrorIcon;
}

[Serializable]
public class DatabaseContent
{
    public VisualDatabase Visuals;

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Buff> Buffs = new List<Buff>();

    public List<ItemData> Items = new List<ItemData>();

    public List<ItemType> Equips = new List<ItemType>();
    
    public List<ItemTypeOverride> EquipSlotOverrides = new List<ItemTypeOverride>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public List<InteractableData> Interactables = new List<InteractableData>();
    
    public List<Expedition> Expeditions = new List<Expedition>();
    
    public List<int> ExpChart = new List<int>();

    public List<string> TimePhases = new List<string>();

    public string StartingScene;

    public string HumanPrefab;

    public AttributeData BaseAttributes;
    
    public AttributeData AttributesLimits;

    public int MaxLevel;

    public int AbilitiesInitCount;

    public int AbilitiesMaxCount;

    public int MaxInventorySlots;
    public int MaxChatLength;
    public int RollDurationSeconds;
    public float LongRangeDistance;
    public float ShortRangeDistance;
    public float IdleSecondsForInvulnerability;
    public int MaxCharacters;
    public int MaxNameLength;
    public int MinNameLength;
    public int HpRegenSeconds;
    public int MaxPartyMembers;
    public int PartyInviteTimeoutSeconds;
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

    [JsonIgnore]
    public string objectiveDescription;

    public string UniqueCompletionScreenEffect;

    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public List<SceneInteractable> Interactables = new List<SceneInteractable>();
    public float playerSpawnX;
    public float playerSpawnY;
    public List<Portal> Portals = new List<Portal>();
    public List<Vendor> Vendors = new List<Vendor>();

    [JsonIgnore]
    public string MusicTrack;

    [JsonIgnore]
    public string Soundscape;

    public bool enablePvp;
}

[Serializable]
public class SceneInteractable
{
    public string interactableName;
    public string interactableId;
    public int positionX;
    public int positionY;

    public SceneInteractable(string name, string id, int x, int y)
    {
        this.interactableName = name;
        this.interactableId = id;
        this.positionX = x;
        this.positionY = y;
    }
}

[Serializable]
public class MobSpawn
{
    public string monsterName;
    public float positionX;
    public float positionY;
    public float respawnSeconds;
    public bool IrellevantMob;
}

[Serializable]
public class ItemTypeOverride
{
    public ItemType itemType;
    public ItemType overrideType;
}
