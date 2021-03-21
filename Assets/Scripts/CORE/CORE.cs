using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    public CanvasGroup GameUICG;

    public Canvas ResolutionDialog;

    public CGDatabase Data;

    public RoomData Room;

    public bool DEBUG = false;

    public bool DEBUG_SPAMMY_EVENTS = false;

    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

    public bool IsBitch;
    public bool InGame = false;
    public bool IsTyping
    {
        get 
        {
            return false
            #if UNITY_EDITOR
            || ConsoleInputUI.Instance.IsTyping
            #endif
            ;
        }
    }
    public bool HasWindowOpen
    {
        get 
        {
            return AbilitiesUI.Instance.IsOpen;
        }
    }

    public bool LongPressMode;
    public bool MoveToHaltMode;
    public bool RepressToHaltMode;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 0.01666667f;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);

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

    }

    private void Update()
    {
        if(InGame && !IsTyping)
        {
            if(Input.GetKeyDown(InputMap.Map["Abilities Window"]) && Room.PlayerActor.ActorEntity)
            {
                AbilitiesUI.Instance.Toggle(Room.PlayerActor.ActorEntity);
            }
            else if (Input.GetKeyDown(InputMap.Map["Character Window"]) && Room.PlayerActor.ActorEntity)
            {
                InventoryUI.Instance.Toggle(SocketHandler.Instance.CurrentUser.actor);
            }
        }
    }

    public void CloseAllUiWindows()
    {
        AbilitiesUI.Instance.Hide();
    }

    public static void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
            container.GetChild(0).SetParent(Instance.transform);
        }
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
       
        actorObject.transform.position = new Vector3(actorData.x,actorData.y,0f);
        actorData.ActorEntity = actorObject.GetComponent<Actor>();
        actorObject.GetComponent<Actor>().SetActorInfo(actorData);

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

        ItemEntity itemEntity =  ResourcesLoader.Instance.GetRecycledObject("ItemEntity").GetComponent<ItemEntity>();
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

    public void ShowScreenEffect(string screenEffectObject)
    {
        GameObject obj = ResourcesLoader.Instance.GetRecycledObject(screenEffectObject);
        obj.transform.SetParent(GameUICG.transform, true);
        obj.transform.position = GameUICG.transform.position;
        obj.transform.localScale = Vector3.one;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

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
            if (CORE.Instance.Room.Actors[i].ActorEntity == from)
            {
                continue;
            }

            if (CORE.Instance.Room.Actors[i].ActorEntity.IsDead)
            {
                continue;
            }

            // If need a player but found mob, or need a mob and found player - continue.
            if (LookForPlayer == CORE.Instance.Room.Actors[i].isMob)
            {
                continue;
            }

            // The from actor must be facing the target.
            bool IsActorToRight = from.transform.position.x < CORE.Instance.Room.Actors[i].ActorEntity.transform.position.x;
            if (from.State.Data.faceRight != IsActorToRight)
            {
                continue;
            }

            float currentDist = Vector2.Distance(from.transform.position, CORE.Instance.Room.Actors[i].ActorEntity.transform.position);
            if (IsNearest = currentDist < nearestDist)
            {
                nearestDist = currentDist;
                nearestActor = CORE.Instance.Room.Actors[i].ActorEntity;
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

        if(PlayerActor == actor)
        {
            PlayerActor = null;
            CORE.Instance.CloseAllUiWindows();
        }

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
