using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    public CanvasGroup GameUICG;

    public Canvas ResolutionDialog;

    public CGDatabase Data;

    public SceneInfo ActiveSceneInfo
    {
        get
        {
            return CORE.Instance.Data.content.Scenes.Find(X => SceneManager.GetActiveScene().name == X.sceneName);
        }
    }

    public RoomData Room;

    public bool DEBUG = false;

    public bool DEBUG_SPAMMY_EVENTS = false;

    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

    public bool IsBitch;
    public bool InGame = false;
    public bool IsLoading = false;
    public bool IsTyping
    {
        get 
        {
            GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGameObject != null)
            {
                TMP_InputField inputField = currentSelectedGameObject.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    if (inputField.isFocused)
                    {
                        return true;
                    }
                }
            }
            return ConsoleInputUI.Instance.IsTyping;
        }
    }
    public bool HasWindowOpen
    {
        get 
        {
            return CurrentWindow != null;
        }
    }

    public WindowInterface CurrentWindow;
    public Dictionary<WindowInterface, KeyCode> WindowToKeyMap = new Dictionary<WindowInterface, KeyCode>();

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.01666667f;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
        LeaveGame();
    }

    private void Start()
    {
        SubscribeToEvent("ActorDied", () => {Room.RefreshThreat();});
        SubscribeToEvent("ActorResurrected", () => {Room.RefreshThreat();});
        SubscribeToEvent("ActorAddedBuff", () => {Room.RefreshThreat();});
        SubscribeToEvent("ActorRemovedBuff", () => {Room.RefreshThreat();});

#if !UNITY_EDITOR
        DelayedInvokation(3f, () =>
        {
            ResolutionDialog.enabled = true;
        });
#endif

        WindowToKeyMap.Add(AbilitiesUI.Instance, InputMap.Map["Abilities Window"]);
        WindowToKeyMap.Add(InventoryUI.Instance, InputMap.Map["Character Window"]);
    }

    private void Update()
    {
        if(InGame && !IsLoading && !IsTyping)
        {
            foreach (var windowToKeyCode in WindowToKeyMap)
            {
                if(Input.GetKeyDown(windowToKeyCode.Value))
                {
                    bool isTargetWindowClosed = CurrentWindow != windowToKeyCode.Key;
                    CloseCurrentWindow();
                    if (isTargetWindowClosed)
                    {
                        CurrentWindow = windowToKeyCode.Key;
                        CurrentWindow.Show(SocketHandler.Instance.CurrentUser.actor);
                    }
                }
            }
            
            if (Input.GetKeyDown(InputMap.Map["Exit"]))
            {
                CloseCurrentWindow();
            }
        }
    }

    public void CloseCurrentWindow()
    {
        if (CurrentWindow != null)
        {
            CurrentWindow.Hide();
            CurrentWindow = null;
        }
    }

    public static void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
            container.GetChild(0).SetParent(Instance.transform);
        }
    }

    private class DisplayAttribute
    {
        public FieldInfo FieldInfo;
        public string SpriteName;
        public string Name;
        public string Description;

        public DisplayAttribute(FieldInfo fieldInfo, string spriteName, string name, string description) {
            this.FieldInfo = fieldInfo;
            this.SpriteName = spriteName;
            this.Name = name;
            this.Description = description;
        }
    }
    private static Dictionary<string, DisplayAttribute> displayAttributes = new Dictionary<string, DisplayAttribute>()
    {
        { "Power", new DisplayAttribute(typeof(AttributeData).GetField("Power"), "icat_9", "Damage","")},
        { "HP", new DisplayAttribute(typeof(AttributeData).GetField("HP"), "icat_0", "HP","")},
        { "Defense", new DisplayAttribute(typeof(AttributeData).GetField("Defense"), "icat_10", "Defense","")},
        { "CDReduction", new DisplayAttribute(typeof(AttributeData).GetField("CDReduction"), "icat_8", "Cooldown Reduction","")},
        { "CTReduction", new DisplayAttribute(typeof(AttributeData).GetField("CTReduction"), "icat_7", "Casting Time Reduction","")},
        { "Lifesteal", new DisplayAttribute(typeof(AttributeData).GetField("Lifesteal"), "", "Lifesteal","")},
        { "LongRangeMultiplier", new DisplayAttribute(typeof(AttributeData).GetField("LongRangeMultiplier"), "crosshair", "Long Range Damage","")},
        { "ShortRangeMultiplier", new DisplayAttribute(typeof(AttributeData).GetField("ShortRangeMultiplier"), "icat_6", "Short Range Damage","")},
        { "WildMagicChance", new DisplayAttribute(typeof(AttributeData).GetField("WildMagicChance"), "icat_5", "Wild Magic","")},
        { "SpellDuration", new DisplayAttribute(typeof(AttributeData).GetField("SpellDuration"), "icat_4", "Casted Buffs Duration","")},
        { "AntiDebuff", new DisplayAttribute(typeof(AttributeData).GetField("AntiDebuff"), "icat_3", "Anti Debuff","")},
        { "Threat", new DisplayAttribute(typeof(AttributeData).GetField("Threat"), "icat_2", "Threat","")},
        { "MovementSpeed", new DisplayAttribute(typeof(AttributeData).GetField("MovementSpeed"), "icat_1", "Movement Speed","")},
        { "DoubleCast", new DisplayAttribute(typeof(AttributeData).GetField("DoubleCast"), "", "Double Cast","")},
        { "StunOnDmg", new DisplayAttribute(typeof(AttributeData).GetField("StunOnDmg"), "", "Stun On Dmg","")},
        { "SpawnSlime", new DisplayAttribute(typeof(AttributeData).GetField("SpawnSlime"), "", "Spawn A Slime","")},
        { "Explode", new DisplayAttribute(typeof(AttributeData).GetField("Explode"), "", "Explode","")},
        { "HpRegen", new DisplayAttribute(typeof(AttributeData).GetField("HpRegen"), "", "HP Regen","")},
    };

    public static string GetTooltipTextFromAttributes(AttributeData data)
    {
        string result = "";

        // First get all the positives, then the negatives.
        foreach (KeyValuePair<string, DisplayAttribute> keyValuePair in displayAttributes)
        {
            float propertyValue = (float)keyValuePair.Value.FieldInfo.GetValue(data);
            
            if (propertyValue > 0)
            {
                string icon = string.IsNullOrEmpty(keyValuePair.Value.SpriteName) ? "" : "<sprite name=\"" + keyValuePair.Value.SpriteName + "\">  ";
                result += Environment.NewLine + "<color=#8AFD97>" + icon + keyValuePair.Value.Name + " +" + Mathf.RoundToInt(propertyValue * 100)+"%" + "</color>";
            }
        }
        foreach (KeyValuePair<string, DisplayAttribute> keyValuePair in displayAttributes)
        {
            float propertyValue = (float)keyValuePair.Value.FieldInfo.GetValue(data);
            
            if (propertyValue < 0)
            {
                string icon = string.IsNullOrEmpty(keyValuePair.Value.SpriteName) ? "" : "<sprite name=\"" + keyValuePair.Value.SpriteName + "\" tint=1>  ";
                result += Environment.NewLine + "<color=#F28B7D>" + icon + keyValuePair.Value.Name + " " + Mathf.RoundToInt( propertyValue * 100)+"%" + "</color>";
            }
        }

        return result;
    }

    public void SubscribeToEvent(string eventKey, UnityAction action)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].AddListener(action);
    }

    public void UnsubscribeFromEvent(string eventKey, UnityAction action)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            Debug.LogError("EVENT " + eventKey + " does not exist!");
            return;
        }

        DynamicEvents[eventKey].RemoveListener(action);
    }

    public void InvokeEvent(string eventKey)
    {
        if (DEBUG)
        {
            Debug.Log("CORE - Event Invoked " + eventKey);
        }

        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].Invoke();
    }

    public void DelayedInvokation(float time, Action action)
    {
        StartCoroutine(DelayedInvokationRoutine(time, action));
    }

    IEnumerator DelayedInvokationRoutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }

    public void ConditionalInvokation(Predicate<object> condition, Action action)
    {
        StartCoroutine(ConditionalInvokationRoutine(condition, action));    
    }

    IEnumerator ConditionalInvokationRoutine(Predicate<object> condition, Action action)
    {
        while(!condition(null))
        {
            yield return new WaitForSeconds(1f);
        }

        action.Invoke();
    }

    public void LogMessage(string message)
    {
        if(!DEBUG)
        {
            return;
        }

        Debug.Log(message);
    }

    public void LogMessageError(string message)
    {
        if (!DEBUG)
        {
            return;
        }

        Debug.LogError(message);
    }

    public void LoadScene(string sceneKey, Action onComplete = null)
    {
        if(LoadSceneRoutineInstance != null)
        {
            StopCoroutine(LoadSceneRoutineInstance);
        }
        Room = new RoomData();
        
        LoadSceneRoutineInstance = StartCoroutine(LoadSceneRoutine(sceneKey,onComplete));
    }

    public void SpawnActor(ActorData actorData)
    {
        GameObject actorObject;

        actorObject = ResourcesLoader.Instance.GetRecycledObject(actorData.prefab);
       
        Vector3 startPos = new Vector3(actorData.x, actorData.y, 0f);
        actorObject.transform.position = startPos;
        actorData.ActorEntity = actorObject.GetComponent<Actor>();
        actorData.ActorEntity.Rigid.position = startPos;

        actorData.ActorEntity.SetActorInfo(actorData);

        Room.ActorJoined(actorData);
    }

    public void DespawnActor(string actorId)
    {
        Room.ActorLeft(actorId);
    }

    public void SpawnInteractable(Interactable interactable)
    {
        InteractableData dataRef = Data.content.Interactables.Find(X => X.name == interactable.interactableName);

        if(dataRef == null)
        {
            LogMessageError("No known interactable " + interactable.interactableName);
            return;
        }

        if (!string.IsNullOrEmpty(dataRef.InteractablePrefab))
        {
            GameObject interactableObject;

            interactableObject = ResourcesLoader.Instance.GetRecycledObject(dataRef.InteractablePrefab);

            interactableObject.transform.position = new Vector3(interactable.x, interactable.y, 0f);
            interactable.Entity = interactableObject.GetComponent<InteractableEntity>();
            interactable.Entity.SetInfo(interactable);
        }

        Room.InteractableJoined(interactable);
    }

    public void DespawnInteractable(string interactableId)
    {
        Room.InteractableLeft(interactableId);
    }

    public void InteractableUse(string interactableId, string byActorID = "")
    {
        Interactable interactable = Room.Interactables.Find(x => x.interactableId == interactableId);

        if(interactable == null)
        {
            LogMessageError("No interactable with the id " + interactableId);
            return;
        }

        interactable.Entity.Interacted(byActorID);
    }

    public void SpawnItem(Item item)
    {
        ///TODO ADD ENTITY

        ItemEntity itemEntity =  ResourcesLoader.Instance.GetRecycledObject("WorldItem").GetComponent<ItemEntity>();
        itemEntity.transform.position = new Vector2(item.x, item.y);
        item.Entity = itemEntity;
        itemEntity.SetInfo(item);

        Room.ItemJoined(item);
    }

    public void DespawnItem(string itemId)
    {
        Room.ItemLeft(itemId);
    }

    Coroutine LoadSceneRoutineInstance;
    IEnumerator LoadSceneRoutine(string sceneKey, Action onComplete = null)
    {
        string currentSceneKey = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneKey);

        if(RoomUpdateRoutineInstance != null)
        {
            StopCoroutine(RoomUpdateRoutineInstance);
            RoomUpdateRoutineInstance = null;
        }

        yield return 0;

        while (SceneManager.GetActiveScene().name != sceneKey)
        {
            yield return 0;
        }

        yield return 0;

        RoomUpdateRoutineInstance = StartCoroutine(RoomUpdateRoutine());

        onComplete?.Invoke();

        EnterGame();

        InvokeEvent("NewSceneLoaded");

        if (sceneKey == "PRELOADER")
        {
            SocketHandler.Instance.SendDisconnectSocket();
            Destroy(this.gameObject);
        }

        LoadSceneRoutineInstance = null;

    //    ObjectiveUI.Instance.SetInfo(Data.content.Scenes.Find(X => X.sceneName == sceneKey).objectiveDescription);

    }

    void EnterGame()
    {
        GameUICG.alpha = 1f;
        InGame = true;
    }

    void LeaveGame()
    {
        GameUICG.alpha = 0f;
        InGame = false;
    }

    Coroutine RoomUpdateRoutineInstance;
    IEnumerator RoomUpdateRoutine()
    {
        while(true)
        {
            Room.SendActorsPositions();
            yield return 0;
        }
    }

    public GameObject ShowScreenEffect(string screenEffectObject, object data = null)
    {
        GameObject obj = ResourcesLoader.Instance.GetRecycledObject(screenEffectObject);
        obj.transform.SetParent(GameUICG.transform, true);
        obj.transform.position = GameUICG.transform.position;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<ScreenEffectUI>().Show(data);

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        return obj;

    }

    public void ActivateParams(List<AbilityParam> onExecuteParams, Actor casterActor = null, Actor originCaster = null)
    {
        foreach (AbilityParam param in onExecuteParams)
        {
            if (param.Type.name == "Movement")
            {
                if ((param.Targets == TargetType.Self || param.Targets == TargetType.FriendsAndSelf) && casterActor != null)
                {
                    if (casterActor != null)
                    {
                        casterActor.ExecuteMovement(param.Value, casterActor);
                    }
                }
                else
                {
                    originCaster.ExecuteMovement(param.Value, casterActor);
                }
            }
            else if (param.Type.name == "Reset Last CD")
            {
                if (originCaster != null)
                {
                    if (originCaster .LastAbility == null)
                    {
                        continue;
                    }

                    originCaster.State.Abilities.Find(x => x.CurrentAbility.name == (originCaster.LastAbility.name)).CurrentCD = 0f;
                }
            }
            else if (param.Type.name == "Reset All CD")
            {
                if (originCaster != null)
                {
                    originCaster.State.Abilities.ForEach(x => x.CurrentCD = 0f);
                }
            }
            else if (param.Type.name == "Start Flying")
            {
                if (originCaster != null)
                {
                    originCaster.StartFlying();
                }
            }
            else if (param.Type.name == "Stop Flying")
            {
                if (originCaster != null)
                {
                    originCaster.StopFlying();
                }
            }
            else if (param.Type.name == "Execute Ability")
            {
                if (originCaster != null)
                {
                    originCaster.AttemptExecuteAbility(CORE.Instance.Data.content.Abilities.Find(x => x.name == param.Value), casterActor);
                }
            }
        }
    }
}

[Serializable]
public class RoomData
{
    public List<ActorData> Actors = new List<ActorData>();
    public List<Interactable> Interactables = new List<Interactable>();
    public List<Item> Items = new List<Item>();

    public ActorData PlayerActor;

    public Dictionary<string, int> RoomStates = new Dictionary<string, int>();

    [JsonIgnore]
    public Actor MostThreateningActor;

    [JsonIgnore]
    public Actor LeastThreatheningActor;

    [JsonIgnore]
    public bool HasEnemies
    {
        get
        {
            return Actors.Find(x=>x.isMob && x.ActorEntity != null && !x.ActorEntity.IsDead) != null;
        }
    }

    public Actor GetMostThreateningActor()
    {
        Actor mostThreatAct = null;
        float mostThreat = Mathf.NegativeInfinity;
        for(int i=0;i<Actors.Count;i++)
        {
            if (Actors[i].ActorEntity.IsDead || Actors[i].isMob)
            {
                continue;
            }

            float currentThreat = Actors[i].ActorEntity.State.Data.attributes.Threat;

            if (currentThreat > mostThreat)
            {
                mostThreatAct = Actors[i].ActorEntity;
                mostThreat = currentThreat;
            }
        }

        return mostThreatAct;
        
    }
    
    public Actor GetLeastThreateningActor()
    {
        Actor leastThreatAct = null;
        float leastThreat = Mathf.Infinity;
        for (int i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].ActorEntity.IsDead || Actors[i].isMob)
            {
                continue;
            }

            float currentThreat = Actors[i].ActorEntity.State.Data.attributes.Threat;

            if (currentThreat < leastThreat)
            {
                leastThreatAct = Actors[i].ActorEntity;
                leastThreat = currentThreat;
            }
        }

        return leastThreatAct;
    }

    public Actor GetFurthestActor(Actor from, bool LookForPlayer)
    {
        return GetDistancedActor(from, LookForPlayer, false);
    }

    public Actor GetNearestActor(Actor from, bool LookForPlayer)
    {
        return GetDistancedActor(from, LookForPlayer, true);
    }

    public Actor GetDistancedActor(Actor from, bool LookForPlayer, bool IsNearest)
    {
        float nearestDist = IsNearest ? Mathf.Infinity : 0;
        Actor nearestActor = null;
        for (int i = 0; i < CORE.Instance.Room.Actors.Count; i++)
        {
            Actor to = CORE.Instance.Room.Actors[i].ActorEntity;
            if (to == from)
            {
                continue;
            }

            if (to.IsDead || to.State.Data.MaxHP == 0)
            {
                continue;
            }

            // If need a player but found mob, or need a mob and found player - continue.
            if (LookForPlayer == to.State.Data.isMob)
            {
                continue;
            }

            // The from actor must be facing the target.
            bool IsActorToRight = from.transform.position.x < to.transform.position.x;
            if (from.State.Data.faceRight != IsActorToRight)
            {
                continue;
            }

            // The to actor is too far down/up
            float yDistance = Mathf.Abs(from.transform.position.y - to.transform.position.y);
            if (yDistance > 12)
            {
                continue;
            }

            float currentDist = Vector2.Distance(from.transform.position, to.transform.position);
            if (IsNearest == currentDist < nearestDist)
            {
                nearestDist = currentDist;
                nearestActor = to;
            }
        }

        return nearestActor;
    }

    public void ActorJoined(ActorData actor)
    {

        Actors.Add(actor);

        if(actor.IsPlayer)
        {
            PlayerActor = actor;
            DisplayEXPEntityUI.Instance.Init();
        }

        if (!actor.isMob)
        {
            RefreshThreat();
        }
    }

    public void ActorLeft(string actorID)
    {
        ActorData actor = Actors.Find(x => x.actorId == actorID);

        if(actor == null)
        {
            CORE.Instance.LogMessageError("No actorId " + actorID + " in room.");
            return;
        }

        CORE.Destroy(actor.ActorEntity.gameObject);

        Actors.Remove(actor);

        if (!actor.isMob)
        {
            RefreshThreat();
        }
    }

    public void InteractableJoined(Interactable interactable)
    {
        Interactables.Add(interactable);
    }

    public void InteractableLeft(string interactableId)
    {
        Interactable interactable = Interactables.Find(x => x.interactableId == interactableId);


        if (interactable == null)
        {
            CORE.Instance.LogMessageError("No interactableId " + interactableId + " in room.");
            return;
        }
        
        Interactables.Remove(interactable);

        CORE.Instance.ConditionalInvokation(
            x => 
        {
            return !interactable.Entity.IsBusy;
        }, () => 
        {
            CORE.Destroy(interactable.Entity.gameObject);
        });
    }

    public void ItemJoined(Item item)
    {
        Items.Add(item);
    }

    public void ItemLeft(string itemId)
    {
        Item item = Items.Find(x => x.itemId == itemId);


        if (item == null)
        {
            CORE.Instance.LogMessageError("No itemId " + itemId + " in room.");
            return;
        }

        Items.Remove(item);

        CORE.Destroy(item.Entity.gameObject);
    }


    public void RefreshThreat()
    {
        MostThreateningActor = GetMostThreateningActor();
        LeastThreatheningActor = GetLeastThreateningActor();
    }

    public void SendActorsPositions()
    {
        List<ActorData> actorsToUpdate = new List<ActorData>();
        JSONNode node = new JSONClass();
        for(int i=0;i<Actors.Count;i++)
        {
            ActorData actor = Actors[i];
            if ((actor.IsPlayer || (!actor.isCharacter && CORE.Instance.IsBitch)) && actor.ActorEntity != null) 
            {
                float lastX = actor.x;
                float lastY = actor.y;
                bool lastFaceRight = actor.faceRight;
                actor.x = actor.ActorEntity.transform.position.x;
                actor.y = actor.ActorEntity.transform.position.y;
                actor.faceRight = actor.ActorEntity.Body.localScale.x < 0f;

                if (lastX != actor.x || lastY != actor.y || lastFaceRight != actor.faceRight) {
                    actorsToUpdate.Add(actor);
                }
            }
        }

        for(int i=0;i<actorsToUpdate.Count;i++)
        {
            ActorData actor = actorsToUpdate[i];
            node["actorPositions"][i]["actorId"] = actor.actorId;
            node["actorPositions"][i]["x"] = actor.x.ToString();
            node["actorPositions"][i]["y"] = actor.y.ToString();
            node["actorPositions"][i]["faceRight"].AsBool = actor.faceRight;
        }

        if (actorsToUpdate.Count > 0)
        {
            SocketHandler.Instance.SendEvent("actors_moved", node);
        }
    }

    public void ReceiveActorPositions(JSONNode data)
    {
        for(int i=0;i<data["actorPositions"].Count;i++)
        {
            ActorData actor = Actors.Find(x => x.actorId == data["actorPositions"][i]["actorId"].Value);

            if(actor == null)
            {
                CORE.Instance.LogMessageError("No actor with id " + data["actorPositions"][i]["actorId"].Value);
                continue;
            }

            actor.x = float.Parse(data["actorPositions"][i]["x"]);
            actor.y = float.Parse(data["actorPositions"][i]["y"]);
            actor.faceRight = bool.Parse(data["actorPositions"][i]["faceRight"]);
        }
    }

}
