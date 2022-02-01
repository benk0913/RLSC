using I2.Loc;
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
    public ServerEnvironment ServerEnvironment;

    public string unic0rn
    {
        get
        {
            return ServerEnvironment.unic0rn;
        }
    }

    public string DataPath = "Data";

    public DatabaseContent content;

    [JsonIgnore]
    public Material DefaultSpriteMaterial;

    [JsonIgnore]
    public Sprite ErrorIcon;

    [JsonIgnore]
    public LanguageSourceAsset Localizator;
}

[Serializable]
public class DatabaseContent
{
    [JsonIgnore]
    public List<GEPlayDialog> temporaryData = new List<GEPlayDialog>();

    [JsonIgnore]
    public List<RealmData> Realms = new List<RealmData>();

    public List<RealmCapacity> RealmsCapacity = new List<RealmCapacity>();

    public int RealmMaxConnectedPeople;

    public int RealmCap = 2;

    public VisualDatabase Visuals;

    public CashShopDatabase CashShop;

    public List<ClassJob> Classes = new List<ClassJob>();

    public List<Ability> Abilities = new List<Ability>();

    public List<Buff> Buffs = new List<Buff>();

    public List<State> States = new List<State>();

    public List<ItemData> Items = new List<ItemData>();

    public List<ItemType> Equips = new List<ItemType>();
    
    public List<ItemTypeOverride> EquipSlotOverrides = new List<ItemTypeOverride>();

    [JsonIgnore]
    public List<Emote> Emotes = new List<Emote>();

    public List<SceneInfo> Scenes = new List<SceneInfo>();

    public List<InteractableData> Interactables = new List<InteractableData>();
    
    public List<Expedition> Expeditions = new List<Expedition>();
    
    public List<int> ExpChart = new List<int>();

    public List<TimePhase> TimePhases = new List<TimePhase>();

    public List<AchievementData> Achievements = new List<AchievementData>();
    public List<QuestData> Quests = new List<QuestData>();
    
    [JsonIgnore]
    public AlignmentData alignmentData;
    public List<MoneyValueMap> MoneyValueMaps = new List<MoneyValueMap>();
    public string LatestVersion;

    public string StartingScene;

    public string TownScene = "Sunset Port";

    public string HumanPrefab;

    public AttributeData BaseAttributes;

    public AttributeData MobsBaseAttributes;
    
    public AttributeData AttributesLimits;

    public List<ItemData> BaseEmotes = new List<ItemData>();

    public ItemData DefaultNameTag;
    public ItemData DefaultChatBubble;

    [JsonIgnore]
    public string titleScreenMusic;
    
    [JsonIgnore]
    public string titleScreenSoundscape;

    public int MaxLevel;

    public int AbilitiesInitCount;

    public int AbilitiesMaxCount;

    public InventoryLikeDatabase InventoryData;
    public InventoryLikeDatabase BankData;

    public int MaxCashItemSlots;
    public int MaxChatLength;
    public int RollDurationSeconds;
    public float LongRangeDistance;
    public float ShortRangeDistance;
    public float IdleSecondsForInvulnerability;
    public int MaxCharacters;
    public int MaxAdditionalCharacters;
    public int MaxNameLength;
    public int MinNameLength;
    public int HpRegenSeconds;
    public float HpRegen;
    public int MaxPartyMembers;
    public int PartyInviteTimeoutSeconds;
    public float OnHitBonusesInternalCdSeconds;
    public int NpcItemsPoolLength;
    public int NpcItemsRefreshMinutes;
    public int ExpeditionQueueMatchDurationSeconds;
    public int TaxiPrice = 50;
    public int BaseCoinDrop = 1;
    public float ExpeditionCoinMultiplier = 5;
    public float PlayerDeathCoinPercent = 0.05f;
    public int BaseExp = 1;
    public float ExpeditionExpMultiplier = 0;

    public int MaxItemStack = 100;

    public int ScrapCost = 30;
    public SlotsDatabase Slots;

    public List<PromoCode> Promos;

    public FriendsData Friends;
    
    public GuildsData Guilds;
    
    public List<RarirtyToScrap> RarirtyToScrapGains; 

    public bool DangerousEveryoneIsAdmin;
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

    // Values that new characters can start with.

    public ActorLooks AvailbleBodyParts;
    
    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultHair = new List<SkinSet>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultEyebrows = new List<SkinSet>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultEyes = new List<SkinSet>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultEars = new List<SkinSet>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultNose = new List<SkinSet>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultMouth = new List<SkinSet>();

    [JsonConverter(typeof(ColorHexConverter))]
    public List<Color> DefaultSkinColor = new List<Color>();

    [JsonConverter(typeof(ColorHexConverter))]
    public List<Color> DefaultHairColor = new List<Color>();

    [JsonConverter(typeof(ObjectNameConverter))]
    public List<SkinSet> DefaultIris = new List<SkinSet>();
}

[Serializable]
public class CashShopDatabase
{
    [JsonIgnore]
    public List<CashShopStore> CashShopStores = new List<CashShopStore>();

    [Serializable]
    public class CashShopStore
    {
        public string StoreKey;
        public List<ItemData> StoreItems = new List<ItemData>();
    }
    
    public List<EQPPrice> Prices = new List<EQPPrice>();

    public string SteamItemDescription;

    [Serializable]
    public class EQPPrice
    {
        public float CostInUSD;
        public int EQPValue;
    }

    public string PlayPackageName;
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

    [JsonIgnore]
    public string UniqueCompletionScreenEffect;

    public List<MobSpawn> Mobs = new List<MobSpawn>();
    public List<SceneInteractable> Interactables = new List<SceneInteractable>();
    public float playerSpawnX;
    public float playerSpawnY;
    public List<Portal> Portals = new List<Portal>();
    public List<VendorData> Vendors = new List<VendorData>();
    public List<OnDemandParams> OnDemandParams = new List<OnDemandParams>();

    public string RespawnMap = "Sunset Port";

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

    [JsonIgnore]
    public string Map;

    [JsonIgnore]
    public string MapPoint;

    
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
    public bool ShuffleItems = true;
}

[Serializable]
public class Vendor
{
    public string id;
    public List<Item> itemsPool = new List<Item>();
}

[Serializable]
public class RealmCapacity
{
    public float Percent;
    public string Text;

    [JsonIgnore]
    public Color Color;
}

[Serializable]
public class TimePhase
{
    public string Name;
    public int DurationInMinutes;
}

[Serializable]
public class SlotsDatabase
{
    public int SlotMachinePrice = 5;

    public List<AbilityParam> Rewards;
}

[Serializable]
public class InventoryLikeDatabase
{
    public int StartingSlots;
    public int MaxSlots;
    public int IncreaseSlotsAmount;
    public List<int> BuySlotsPrices;
}

[Serializable]
public class FriendsData
{
    public int FriendRequestTimeoutSeconds;
}

[Serializable]
public class GuildsData
{
    public int CreateGuildCostScraps;

    public int GuildInviteTimeoutSeconds;

    public int GuildNameLengthMin;
    
    public int GuildNameLengthMax;

    public List<int> UpgradeCosts;

    public List<GuildUpgrade> Upgrades;   
}

[Serializable]
public class GuildUpgrade
{
    public string Key;
    public string DisplayName;
}

[Serializable]
public class RarirtyToScrap
{
    public ItemRarity Rarity;

    public int ScrapGained;

    [JsonConverter(typeof(ObjectNameConverter))]
    public ItemData ScrapItemGained;
}