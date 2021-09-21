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
    public List<ItemData> temporaryData = new List<ItemData>();
    public VisualDatabase Visuals;

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Buff> Buffs = new List<Buff>();

    public List<State> States = new List<State>();

    public List<ItemData> Items = new List<ItemData>();

    public List<ItemType> Equips = new List<ItemType>();
    
    public List<ItemTypeOverride> EquipSlotOverrides = new List<ItemTypeOverride>();

    public List<Emote> Emotes = new List<Emote>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public List<InteractableData> Interactables = new List<InteractableData>();
    
    public List<Expedition> Expeditions = new List<Expedition>();
    
    public List<int> ExpChart = new List<int>();

    public List<string> TimePhases = new List<string>();
    
    public AlignmentData alignmentData;
    public List<MoneyValueMap> MoneyValueMaps = new List<MoneyValueMap>();
    public string LatestVersion;

    public string StartingScene;

    public string HumanPrefab;

    public AttributeData BaseAttributes;

    public AttributeData MobsBaseAttributes;
    
    public AttributeData AttributesLimits;

    public List<ItemData> BaseEmotes = new List<ItemData>();
    public ItemData DefaultNameTag;
    public ItemData DefaultChatBubble;

    public int MaxLevel;

    public int AbilitiesInitCount;

    public int AbilitiesMaxCount;

    public int MaxInventorySlots;
    public int MaxCashItemSlots;
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
    public int TimePhasesDurationMinutes;
    public int NpcItemsPoolLength;
    public int NpcItemsRefreshMinutes;
    public int ExpeditionQueueMatchDurationSeconds;

    public int BaseCoinDrop = 5;
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
public class AlignmentData
{
    public int MaxKarma = 100;
    public List<AlignmentAbility> GoodAbilities = new List<AlignmentAbility>();
    public List<AlignmentAbility> EvilAbilities = new List<AlignmentAbility>();
}


[Serializable]
public class SceneInfo
{
    public string sceneName;

    [JsonIgnore]
    public string displyName;

    [JsonIgnore]
    public string objectiveDescription;

    public string UniqueCompletionScreenEffect;

    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public List<SceneInteractable> Interactables = new List<SceneInteractable>();
    public float playerSpawnX;
    public float playerSpawnY;
    public List<Portal> Portals = new List<Portal>();
    public List<VendorData> Vendors = new List<VendorData>();

    [JsonIgnore]
    public string MusicTrack;

    [JsonIgnore]
    public string Soundscape;


       [JsonIgnore]
    public string NightMusicTrack;

    [JsonIgnore]
    public string NightSoundscape;

    public bool enablePvp;

    [JsonIgnore]
    public bool displayTitleOnEnter = false;

    [JsonIgnore]
    public Sprite PredictionImage;
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

[Serializable]
public class MoneyValueMap
{
    [JsonConverter(typeof(ObjectNameConverter))]
    public ItemData moneyItem;
    public int minAmount;
}

[Serializable]
public class VendorData
{
    public string ID;
    public List<ItemData> Items = new List<ItemData>();
}

[Serializable]
public class Vendor
{
    public string id;
    public List<Item> itemsPool = new List<Item>();
}