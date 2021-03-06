﻿using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using EdgeworldBase;
using Newtonsoft.Json;
using PlatformSupport.Collections.ObjectModel;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SocketHandler : MonoBehaviour
{
    #region Essentials

    public static SocketHandler Instance;

    public bool IsLocal = false;
    public string LocalHostUrl = "http://localhost:5000";
    public string LocalSocketUrl = "http://localhost:5000/socket.io/";
    public string ProdHostUrl = "https://lul2.herokuapp.com";
    public string ProdSocketUrl = "https://lul2.herokuapp.com/socket.io/";
    public string HostUrl { get { return IsLocal ? LocalHostUrl : ProdHostUrl; } }
    public string SocketUrl { get { return IsLocal ? LocalSocketUrl : ProdSocketUrl; } }

    public UserData CurrentUser;

    public SocketManager SocketManager;

    public List<SocketEventListener> SocketEventListeners = new List<SocketEventListener>();

    public int UniqueNumber;

    private void Awake()
    {
        Instance = this;

        UniqueNumber = UnityEngine.Random.Range(0,9999);

        SetupSocketIO();
    }

    private void OnApplicationQuit()
    {
        SendDisconnectSocket();
    }

    void SetupSocketIO()
    {
        SocketManager = new SocketManager(new Uri(HostUrl));

        SocketEventListeners.Clear();


        SocketEventListeners.Add(new SocketEventListener("load_scene", OnLoadScene));
        SocketEventListeners.Add(new SocketEventListener("event_error", OnError));

        SocketEventListeners.Add(new SocketEventListener("actor_spawn", OnActorSpawn));
        SocketEventListeners.Add(new SocketEventListener("actor_despawn", OnActorDespawn));
        SocketEventListeners.Add(new SocketEventListener("move_actors", OnMoveActors));

        SocketEventListeners.Add(new SocketEventListener("bitch_please", OnBitchPlease));
        SocketEventListeners.Add(new SocketEventListener("actor_bitch", OnActorBitch));

        SocketEventListeners.Add(new SocketEventListener("actor_prepare_ability", OnActorPrepareAbility));
        SocketEventListeners.Add(new SocketEventListener("actor_execute_ability", OnActorExecuteAbility));
        SocketEventListeners.Add(new SocketEventListener("actor_ability_hit", OnActorAbilityHit));
        SocketEventListeners.Add(new SocketEventListener("actor_ability_miss", OnActorAbilityMiss));

        SocketEventListeners.Add(new SocketEventListener("actor_add_buff", OnActorAddBuff));
        SocketEventListeners.Add(new SocketEventListener("actor_remove_buff", OnActorRemoveBuff));
        SocketEventListeners.Add(new SocketEventListener("actor_set_attributes", OnActorSetAttributes));
        SocketEventListeners.Add(new SocketEventListener("actor_set_states", OnActorSetStates));
        SocketEventListeners.Add(new SocketEventListener("actor_update_data", OnActorUpdateData));

        SocketEventListeners.Add(new SocketEventListener("actor_hurt", OnActorHurt));
        SocketEventListeners.Add(new SocketEventListener("actor_interrupt", OnActorInterrupt));

        SocketEventListeners.Add(new SocketEventListener("interactable_spawn", OnInteractableSpawn));
        SocketEventListeners.Add(new SocketEventListener("interactable_despawn", OnInteractableDespawn));
        SocketEventListeners.Add(new SocketEventListener("interactable_use", OnInteractableUse));
        SocketEventListeners.Add(new SocketEventListener("room_state", OnRoomState));
        SocketEventListeners.Add(new SocketEventListener("room_states", OnRoomStates));
        SocketEventListeners.Add(new SocketEventListener("room_vendors", OnVendorUpdate));

        SocketEventListeners.Add(new SocketEventListener("game_states", OnGameStates));

        SocketEventListeners.Add(new SocketEventListener("exp_update", OnExpUpdate));
        SocketEventListeners.Add(new SocketEventListener("level_up", OnLevelUp));

        SocketEventListeners.Add(new SocketEventListener("expedition_floor_complete", OnExpeditionFloorComplete));

        // Items
        SocketEventListeners.Add(new SocketEventListener("item_spawn", OnItemsSpawn));
        SocketEventListeners.Add(new SocketEventListener("item_despawn", OnItemDespawn));
        SocketEventListeners.Add(new SocketEventListener("actor_update_item_slot", OnUpdateItemSlot));
        SocketEventListeners.Add(new SocketEventListener("actor_update_equip_slot", OnActorUpdateEquipSlot));
        SocketEventListeners.Add(new SocketEventListener("actor_pick_item", OnActorPickItem));
        SocketEventListeners.Add(new SocketEventListener("orb_added", OnOrbAdded));
        SocketEventListeners.Add(new SocketEventListener("money_refresh", OnMoneyRefresh));

        // Rolls
        SocketEventListeners.Add(new SocketEventListener("choose_item_roll", OnChooseItemRoll));
        SocketEventListeners.Add(new SocketEventListener("actor_chose_rolled_item", OnActorChoseRolledItem));
        SocketEventListeners.Add(new SocketEventListener("actor_rolls", OnActorRolls));

        // Chat
        SocketEventListeners.Add(new SocketEventListener("actor_chat_message", OnActorChatMessage));

        // Party
        SocketEventListeners.Add(new SocketEventListener("party_invite", OnPartyInvite));
        SocketEventListeners.Add(new SocketEventListener("party_invite_timeout", OnPartyInviteTimeout));
        SocketEventListeners.Add(new SocketEventListener("party_join", OnPartyJoin));
        SocketEventListeners.Add(new SocketEventListener("party_decline", OnPartyDecline));
        SocketEventListeners.Add(new SocketEventListener("party_leave", OnPartyLeave));
        SocketEventListeners.Add(new SocketEventListener("party_leader", OnPartyLeader));
        SocketEventListeners.Add(new SocketEventListener("party_toggle_offline", OnPartyToggleOffline));
        SocketEventListeners.Add(new SocketEventListener("party_status", OnPartyStatus));

        // Expedition
        SocketEventListeners.Add(new SocketEventListener("expedition_queue_start", OnExpeditionQueueStart));
        SocketEventListeners.Add(new SocketEventListener("expedition_queue_stop", OnExpeditionQueueStop));
        SocketEventListeners.Add(new SocketEventListener("expedition_queue_match_found", OnExpeditionQueueMatchFound));
        SocketEventListeners.Add(new SocketEventListener("expedition_queue_match_hide", OnExpeditionQueueMatchHide));


        foreach (SocketEventListener listener in SocketEventListeners)
        {
            listener.InternalCallback = AddEventListenerLogging + listener.InternalCallback;
        }
    }

    void AddEventListenerLogging(string eventName, JSONNode data)
    {
        if (!CORE.Instance.DEBUG_SPAMMY_EVENTS && (!string.IsNullOrEmpty(data["actorPositions"].ToString()) || !string.IsNullOrEmpty(data["actorMoved"].ToString())))
        {
            return;
        }

        CORE.Instance.LogMessage("On Socket Event: " + eventName + " | " + data.ToString());
    }


    public void AddListeners()
    {


        foreach (SocketEventListener listener in SocketEventListeners)
        {
            SocketManager.Socket.On(listener.EventKey, listener.Callback);
        }

        SocketManager.Socket.On(SocketIOEventTypes.Error, OnErrorRawCallback);
        SocketManager.Socket.On(SocketIOEventTypes.Disconnect, OnDisconnect);
    }

    #endregion



    #region Request

    public void SendLogin(Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Connecting", Color.green, 3f, true));

        JSONNode node = new JSONClass();
        node["skipTutorial"] = SkippedTutorial();
        node["version"] = Application.version;
        
        SendWebRequest(HostUrl + "/login", (UnityWebRequest lreq) =>
        {
            OnLogin(lreq);

            OnComplete?.Invoke();
        },
        node.ToString(),
        null,
        true);
    }

    public void SendCreateCharacter(string element = "fire", ActorData actor = null, Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Creating Character...", Color.green, 1f, true));

        JSONNode node = new JSONClass();
        node["skipTutorial"] = SkippedTutorial();
        node["classJob"] = element;
        node["actor"] = JSON.Parse(JsonConvert.SerializeObject(actor));

        SendWebRequest(HostUrl + "/create-char", (UnityWebRequest ccreq) =>
        {
            OnCreateCharacter(ccreq);


            OnComplete?.Invoke();
        },
        node.ToString(),
        null,
        true);
    }

    public void SendGetRandomName(bool IsFemale, Action<string> OnComplete)
    {
        JSONNode node = new JSONClass();
        node["skipTutorial"] = SkippedTutorial();
        node["isFemale"].AsBool = IsFemale;

        SendWebRequest(HostUrl + "/random-name", (UnityWebRequest ccreq) =>
        {
            JSONNode data = JSON.Parse(ccreq.downloadHandler.text);

            OnComplete.Invoke(data["name"]);
        },
        node.ToString(),
        null,
        true);
    }

    public void SendDeleteCharacter(string actorId, Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Deleting Character...", Color.green, 1f, true));

        JSONNode node = new JSONClass();
        node["skipTutorial"] = SkippedTutorial();
        node["actorId"] = actorId;

        SendWebRequest(HostUrl + "/delete-char", (UnityWebRequest ccreq) =>
        {
            OnDeleteCharacter(ccreq);


            OnComplete?.Invoke();
        },
        node.ToString(),
        null,
        true);
    }

    public void SendSelectCharacter(Action OnComplete = null, int index = 0)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Selecting Character", Color.green, 1f, true));

        CurrentUser.SelectedCharacterIndex = index;
        CurrentUser.actor = CurrentUser.chars[index];
        SendConnectSocket(OnComplete);
    }

    public void SendConnectSocket(Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Embracing Humanity...", Color.green, 3f, true));

        if (ConnectSocketRoutineInstance != null)
        {
            StopCoroutine(ConnectSocketRoutineInstance);
        }

        ConnectSocketRoutineInstance = StartCoroutine(ConnectSocketRoutine(OnComplete));
    }

    Coroutine ConnectSocketRoutineInstance;
    IEnumerator ConnectSocketRoutine(Action OnComplete = null)
    {
        //BestHTTP.HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.All; //Uncomment to log socket...

        SocketOptions options = new SocketOptions();
        options.AdditionalQueryParams = new ObservableDictionary<string, string>();
        options.AdditionalQueryParams.Add("skipTutorial", SkippedTutorial());
        options.AdditionalQueryParams.Add("charIndex", CurrentUser.SelectedCharacterIndex.ToString());

#if UNITY_EDITOR
        options.AdditionalQueryParams.Add("isEditor", "1");
#endif

        options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;

        DisconnectSocket();

        SocketManager = new SocketManager(new Uri(SocketUrl), options);
        SocketManager.Encoder = new SimpleJsonEncoder();

        AddListeners();

        SocketManager.Open();

        while (SocketManager.State != SocketManager.States.Open)
        {
            yield return 0;
        }

        OnComplete?.Invoke();

        CORE.Instance.LogMessage("Connected To Socket.");

        ConnectSocketRoutineInstance = null;
    }

    private string SkippedTutorial()
    {
        return SystemInfo.deviceUniqueIdentifier
#if UNITY_EDITOR
            +"-editor"
#else
            +UniqueNumber+"-devbuild"
#endif

        ;
    }

    public void SendDisconnectSocket()
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Rejecting Humanity... Returning to MONKE...", Color.green, 3f, true));

        DisconnectSocket();

        CORE.Instance.LogMessage("Disconnected From Socket.");
    }

    private void DisconnectSocket()
    {
        if (SocketManager != null)
        {
            SocketManager.Close();
            SocketManager = null;
        }
    }


    #endregion

    #region Response

    public void OnLogin(UnityWebRequest response)
    {
        JSONNode data = JSON.Parse(response.downloadHandler.text);

        CurrentUser.chars = JsonConvert.DeserializeObject<ActorData[]>(data["chars"].ToString());
        // TODO replace entire user data in repsonse?
    }

    public void OnCreateCharacter(UnityWebRequest response)
    {
        //CurrentUser.actor = JsonConvert.DeserializeObject<ActorData>(response.downloadHandler.text);
        JSONNode data = JSON.Parse(response.downloadHandler.text);
    }
    public void OnDeleteCharacter(UnityWebRequest response)
    {
        // TODO update characters list
    }


    #endregion

    #region HTTP Request Handling

    public void SendWebRequest(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null, bool isPost = false)
    {
        StartCoroutine(SendHTTPRequestRoutine(url, OnResponse, sentJson, urlParams, isPost));
    }

    public IEnumerator SendHTTPRequestRoutine(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null, bool isPost = false)
    {

        UnityWebRequest request;

        if (urlParams != null)
        {
            string urlWithParams = url;

            urlWithParams += "?" + urlParams.Keys.ElementAt(0) + "=" + urlParams[urlParams.Keys.ElementAt(0)];

            for (int i = 1; i < urlParams.Keys.Count; i++)
            {
                urlWithParams += "&" + urlParams.Keys.ElementAt(i) + "=" + urlParams[urlParams.Keys.ElementAt(i)];
            }

            if (CORE.Instance.DEBUG)
            {
                CORE.Instance.LogMessage("Request: " + urlWithParams + " | " + sentJson);
            }

            if (isPost)
            {
                request = UnityWebRequest.Post(urlWithParams, new WWWForm());
            }
            else
            {
                request = UnityWebRequest.Get(urlWithParams);
            }
        }
        else
        {
            if (CORE.Instance.DEBUG)
            {
                CORE.Instance.LogMessage("Request: " + url + " | " + sentJson);
            }


            if (isPost)
            {
                request = UnityWebRequest.Post(url, new WWWForm());
            }
            else
            {
                request = UnityWebRequest.Get(url);
            }
        }


        if (!string.IsNullOrEmpty(sentJson))

        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(sentJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");
        }

        yield return request.SendWebRequest();

        if (CORE.Instance.DEBUG)
        {
            CORE.Instance.LogMessage("Response: " + url + " | " + request.downloadHandler.text);
        }

        if (request.isNetworkError || request.isHttpError)
        {
            CORE.Instance.LogMessageError(request.error + " | " + request.downloadHandler.text);
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(request.downloadHandler.text, Color.red, 2f, true));

            yield break;
        }


        OnResponse?.Invoke(request);

    }

    #endregion

    #region Socket Request

    public void SendEvent(string eventKey, JSONNode node = null)
    {
        if (node == null)
        {
            CORE.Instance.LogMessage("Sending Event: " + eventKey);

            node = new JSONClass();
            SocketManager.Socket.Emit(eventKey, node);
            return;
        }

        if (eventKey != "actors_moved" || CORE.Instance.DEBUG_SPAMMY_EVENTS)
        {
            CORE.Instance.LogMessage("Sending Event: " + eventKey + " | " + node.ToString());
        }

        SocketManager.Socket.Emit(eventKey, node);
    }


    #endregion

    #region Socket Response

    public void OnErrorRawCallback(Socket socket, Packet packet, params object[] args)
    {
        Error error = args[0] as Error;

        switch (error.Code)
        {
            case SocketIOErrors.User:
                CORE.Instance.LogMessageError("Exception in an event handler! Message: " + error.Message);
                break;
            case SocketIOErrors.Custom:
                // This error case is when having issues connecting to the game, e.g. when you're already connected on another PC.
                JSONNode errorData = JSON.Parse(error.Message.ToString());
                string errorMessage = errorData["message"];
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(errorMessage, Color.red, 2, true));
                CORE.Instance.LogMessageError("Server custom error. Message: " + errorMessage);
                DisconnectSocket();
                break;
            default:
                CORE.Instance.LogMessageError("Server error!" + " Code: " + error.Code + ". Message: " + error.Message);
                break;
        }
    }

    public void OnDisconnect(Socket socket, Packet packet, params object[] args)
    {
        CORE.Instance.ReturnToMainMenu();
    }

    public void OnError(string eventName, JSONNode data)
    {
        string error = data["error"].Value;
        if (string.IsNullOrEmpty(error))
        {
            error = "Server Error: " + data.ToString();
        }

        int durationInSeconds = data["durationInSeconds"].AsInt > 0 ? data["durationInSeconds"].AsInt : 1;

        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(error, Color.red, durationInSeconds, true));
        CORE.Instance.LogMessageError(error);
    }

    public void OnLoadScene(string eventName, JSONNode data)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Entering " + data["scene"].Value, Color.green, 1f, false));

        CORE.Instance.IsLoading = true;
        CORE.Instance.CloseCurrentWindow();
        ScreenFaderUI.Instance.FadeToBlack(() =>
        {


            CORE.Instance.LoadScene(data["scene"].Value, () =>
            {
                ScreenFaderUI.Instance.FadeFromBlack();
                SendEvent("scene_loaded");
                CORE.Instance.IsLoading = false;

                SceneInfo sceneInfo = CORE.Instance.ActiveSceneInfo;

                CORE.Instance.RefreshSecneInfo();

                if (sceneInfo.displayTitleOnEnter)
                {
                    CORE.Instance.ShowScreenEffect("ScreenEffectLocation", sceneInfo.sceneName);

                }

            });
        });
    }

    public void OnInteractableSpawn(string eventName, JSONNode data)
    {
        CORE.Instance.SpawnInteractable(JsonConvert.DeserializeObject<Interactable>(data["interactable"].ToString()));
    }

    public void OnInteractableDespawn(string eventName, JSONNode data)
    {
        CORE.Instance.DespawnInteractable(data["interactableId"]);
    }

    public void OnInteractableUse(string eventName, JSONNode data)
    {
        CORE.Instance.InteractableUse(data["interactableId"], data["actorId"]);
    }

    public void OnRoomState(string eventName, JSONNode data)
    {
        string State = data["state"].Value;
        int Value = data["value"].AsInt;

        if (!CORE.Instance.Room.RoomStates.ContainsKey(State))
        {
            CORE.Instance.Room.RoomStates.Add(State, Value);
        }

        CORE.Instance.Room.RoomStates[State] = Value;
        CORE.Instance.InvokeEvent("RoomStatesChanged");
    }

    public void OnVendorUpdate(string eventName, JSONNode data)
    {
        CORE.Instance.Room.RefreshVendors(JsonConvert.DeserializeObject<List<Vendor>>(data["vendors"].ToString()));
    }

    public void OnRoomStates(string eventName, JSONNode data)
    {
        CORE.Instance.Room.RoomStates = JsonConvert.DeserializeObject<Dictionary<string, int>>(data["states"].ToString());
        CORE.Instance.InvokeEvent("RoomStatesChanged");
    }

    public void OnGameStates(string eventName, JSONNode data)
    {
        CORE.Instance.GameStates = JsonConvert.DeserializeObject<Dictionary<string, string>>(data["states"].ToString());
        CORE.Instance.InvokeEvent("GameStatesChanged");
    }

    public void OnExpUpdate(string eventName, JSONNode data)
    {
        //int prevExp = CurrentUser.actor.exp;

        CurrentUser.actor.exp = data["exp"].AsInt;

        //if (prevExp > CurrentUser.actor.exp)
        //{
        //    return;
        //}

        DisplayEXPEntityUI.Instance.Show(CurrentUser.actor.exp);
    }

    public void OnLevelUp(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }
        actorDat.level++;
        if (actorDat.IsPlayer)
        {
            int newSpellPosition = actorDat.level - 2 + CORE.Instance.Data.content.AbilitiesInitCount;
            Ability newAbility = CORE.Instance.Data.content.Abilities.Find(x => x.name == actorDat.ClassJobReference.Abilities[newSpellPosition]);

            if (newAbility != null)
            {
                CORE.Instance.ShowScreenEffect("ScreenEffectGainSpell", newAbility);

            }

            if (actorDat.abilities.Count < CORE.Instance.Data.content.AbilitiesMaxCount)
            {
                if (newAbility != null)
                {
                    actorDat.abilities.Add(newAbility.name);
                }

                actorDat.ActorEntity.RefreshAbilities();
            }
            else
            {
                AbilitiesUI.Instance.RefreshUI();
            }
        }

        GameObject lvlUpEffect = ResourcesLoader.Instance.GetRecycledObject("LevelUpEffect");
        lvlUpEffect.transform.position = actorDat.ActorEntity.transform.position;
        lvlUpEffect.GetComponent<AbilityCollider>().SetInfo(null, actorDat.ActorEntity);
    }

    public void OnExpeditionFloorComplete(string eventName, JSONNode data)
    {
        if (data["showEffect"].AsBool)
        {
            SceneInfo currentInfo = CORE.Instance.Data.content.Scenes.Find(X => SceneManager.GetActiveScene().name == X.sceneName);
            if (!string.IsNullOrEmpty(currentInfo.UniqueCompletionScreenEffect))
            {
                CORE.Instance.ShowScreenEffect(currentInfo.UniqueCompletionScreenEffect);
            }
            else
            {
                CORE.Instance.ShowScreenEffect("ScreenEffectChamberComplete");
            }

        }
        CORE.Instance.InvokeEvent("ChamberComplete");
        //ObjectiveUI.Instance.SetInfo("Proceed to the next stage.");
    }

    public void SendEnterPortal(Portal portal)
    {
        JSONNode node = new JSONClass();
        node["portalId"] = portal.name;

        SocketHandler.Instance.SendEvent("entered_portal", node);
    }

    public void SendPickedItem(string itemId)
    {
        JSONNode node = new JSONClass();
        node["itemId"] = itemId;

        SocketHandler.Instance.SendEvent("picked_item", node);
    }

    public void SendDroppedItem(int slotIndex)
    {
        JSONNode node = new JSONClass();
        node["slotIndex"].AsInt = slotIndex;

        SocketHandler.Instance.SendEvent("dropped_item", node);
    }

    public void SendEquippedItem(int slotIndex)
    {
        JSONNode node = new JSONClass();
        node["slotIndex"].AsInt = slotIndex;

        SocketHandler.Instance.SendEvent("equipped_item", node);
    }

    public void SendUnequippedItem(string equipType)
    {
        JSONNode node = new JSONClass();
        node["equipType"] = equipType;

        SocketHandler.Instance.SendEvent("unequipped_item", node);
    }

    public void SendSwappedItemSlots(int slotIndex1, int slotIndex2)
    {
        JSONNode node = new JSONClass();
        node["slotIndex1"].AsInt = slotIndex1;
        node["slotIndex2"].AsInt = slotIndex2;

        SocketHandler.Instance.SendEvent("swapped_item_slots", node);
    }

    public void SendSwappedItemAndEquipSlots(int slotIndex, string equipType)
    {
        JSONNode node = new JSONClass();
        node["equipType"] = equipType;
        node["slotIndex"].AsInt = slotIndex;

        SocketHandler.Instance.SendEvent("swapped_item_and_equip_slots", node);
    }

    public void OnMoveActors(string eventName, JSONNode data)
    {
        CORE.Instance.Room.ReceiveActorPositions(data);
    }

    protected void OnBitchPlease(string eventName, JSONNode data)
    {
        SendEvent("bitch_please", data);
    }

    protected void OnActorBitch(string eventName, JSONNode data)
    {
        CORE.Instance.IsBitch = data["is_bitch"].AsBool;
        CORE.Instance.InvokeEvent("BitchChanged");
    }


    public void OnActorSpawn(string eventName, JSONNode data)
    {
        ActorData actor = JsonConvert.DeserializeObject<ActorData>(data["actor"].ToString());

        if (CurrentUser.actor.actorId == actor.actorId)
        {
            CurrentUser.actor = actor;
        }

        CORE.Instance.SpawnActor(actor);
    }

    public void OnActorDespawn(string eventName, JSONNode data)
    {
        CORE.Instance.DespawnActor(data["actorId"].Value);
    }

    public void OnActorPrepareAbility(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string abilityName = data["abilityName"];

        if (!actorDat.ActorEntity.IsClientControl)
        {
            actorDat.ActorEntity.PrepareAbility(CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName));
        }
    }

    public void OnActorExecuteAbility(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string abilityName = data["abilityName"];

        Ability ability = CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName);
        Vector2 position = new Vector2(data["x"].AsFloat, data["y"].AsFloat);
        bool faceRight = data["faceRight"].AsBool;
        bool castingExternal = data["castingExternal"].AsBool;

        actorDat.ActorEntity.ExecuteAbility(ability, position, faceRight, castingExternal);


    }

    public void OnActorAbilityHit(string eventName, JSONNode data)
    {
        string givenActorId = data["targetActorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["targetActorId"].Value);
            return;
        }

        string casterActorId = data["casterActorId"].Value;
        ActorData casterActorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == casterActorId);

        if (casterActorDat == null)
        {
            CORE.Instance.LogMessageError("No caster actor with ID " + data["casterActorId"].Value);
            //return;
        }

        string abilityName = data["abilityName"];

        Ability ability = CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName);

        actorDat.ActorEntity.HitAbility(casterActorDat.ActorEntity, ability);
    }

    public void OnActorAbilityMiss(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string abilityName = data["abilityName"];

        Ability ability = CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName);

        actorDat.ActorEntity.MissAbility(ability);
    }

    public void OnActorAddBuff(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string buffName = data["buffName"];
        float duration = data["durationInSeconds"].AsFloat;

        Buff buff = CORE.Instance.Data.content.Buffs.Find(x => x.name == buffName);

        actorDat.ActorEntity.AddBuff(buff, duration);


    }

    public void OnActorRemoveBuff(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string buffName = data["buffName"];

        Buff buff = CORE.Instance.Data.content.Buffs.Find(x => x.name == buffName);

        actorDat.ActorEntity.RemoveBuff(buff);

    }

    public void OnActorSetAttributes(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        actorDat.attributes = JsonConvert.DeserializeObject<AttributeData>(data["attributes"].ToString());
        CORE.Instance.InvokeEvent("StatsChanged");
    }

    public void OnActorUpdateData(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        actorDat.hp = data["hp"].AsInt;

    }

    public void OnActorHurt(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string casterActorId = data["casterActorId"].Value;
        ActorData casterActorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == casterActorId);

        if (casterActorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["casterActorId"].Value);
        }

        int dmg = data["dmg"].AsInt;


        if (casterActorDat != null)
        {
            actorDat.ActorEntity.ShowHurtLabel(dmg, casterActorDat.ActorEntity);
            casterActorDat.ActorEntity.AddDps(dmg);
        }
        else
        {
            actorDat.ActorEntity.ShowHurtLabel(dmg);
        }
    }

    public void OnActorInterrupt(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        actorDat.ActorEntity.State.Interrupt(data["putAbilityOnCd"].AsBool, data["putAllAbilitiesOnCd"].AsBool);
    }

    public void OnActorSetStates(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        actorDat.states = JsonConvert.DeserializeObject<Dictionary<string, StateData>>(data["states"].ToString());

        if (actorDat.ActorEntity == null)
        {
            CORE.Instance.LogMessageError("ACTOR DATA HAS NO ENTITY?");
            return;
        }

        actorDat.OnRefreshStates?.Invoke();
        CORE.Instance.InvokeEvent("ActorChangedStates");
    }

    public void OnItemsSpawn(string eventName, JSONNode data)
    {
        Item item = JsonConvert.DeserializeObject<Item>(data["item"].ToString());

        CORE.Instance.SpawnItem(item);
    }

    public void OnItemDespawn(string eventName, JSONNode data)
    {
        CORE.Instance.DespawnItem(data["itemId"].Value);
    }

    public void OnUpdateItemSlot(string eventName, JSONNode data)
    {
        int slotIndex = data["slotIndex"].AsInt;
        Item item = JsonConvert.DeserializeObject<Item>(data["item"].ToString());

        CurrentUser.actor.items[slotIndex] = item;

        InventoryUI.Instance.RefreshUI();
    }

    public void OnActorUpdateEquipSlot(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string equipType = data["equipType"].Value;
        Item item = JsonConvert.DeserializeObject<Item>(data["item"].ToString());

        actorDat.equips[equipType] = item;

        actorDat.ActorEntity.RefreshLooks();

        if (actorDat.IsPlayer)
        {
            InventoryUI.Instance.RefreshUI();
        }
    }

    public void OnActorPickItem(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        Item item = CORE.Instance.Room.Items.Find(x => x.itemId == data["itemId"].Value);

        if (item == null)
        {
            CORE.Instance.LogMessageError("No item with ID " + data["itemId"].Value);
            return;
        }

        item.Entity.BePickedBy(actorDat.ActorEntity);
        if (item.Data.Type.name == "Money")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorDat.name + " has picked up " + String.Format("{0:n0}", item.amount) + " coins</color>");
        }
        else
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorDat.name + " has picked up the item: '" + item.itemName + "'</color>");
        }

        CORE.Instance.IsPickingUpItem = false;
    }

    public void OnOrbAdded(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        Item orb = JsonConvert.DeserializeObject<Item>(data["orb"].ToString());

        actorDat.orbs.Add(orb);
        actorDat.OnRefreshAbilities.Invoke();
        actorDat.ActorEntity.RefreshOrbs();

        if (actorDat.IsPlayer)
        {
            CORE.Instance.InvokeEvent("OrbsChanged");
            CORE.Instance.ShowScreenEffect("ScreenEffectGainOrb", orb.Data);
        }
    }

    public void OnMoneyRefresh(string eventName, JSONNode data)
    {
        int money = data["money"].AsInt;

        CORE.Instance.Room.PlayerActor.money = money;

        InventoryUI.Instance.RefreshUI();
    }

    public void OnChooseItemRoll(string eventName, JSONNode data)
    {
        Item item = JsonConvert.DeserializeObject<Item>(data["item"].ToString());
        float durationInSeconds = data["durationInSeconds"].AsFloat;

        LootRollPanelUI.Instance.AddLootRollItem(item, durationInSeconds);
    }

    public void OnActorChoseRolledItem(string eventName, JSONNode data)
    {
        Item item = JsonConvert.DeserializeObject<Item>(data["item"].ToString());

        if (item == null)
        {
            CORE.Instance.LogMessageError("No item!");
            return;
        }

        string error = data["error"].Value;
        if (error == "Inventory is full")
        {
            LootRollPanelUI.Instance.ReleaseLootRollItem(item);
            return;
        }
    }

    public void OnActorRolls(string eventName, JSONNode data)
    {
        Item rolledItem = JsonConvert.DeserializeObject<Item>(data["item"].ToString());

        if (rolledItem == null)
        {
            CORE.Instance.LogMessageError("No rolled item");
            return;
        }

        LootRollPanelUI.Instance.RemoveLootRollItem(rolledItem);

        ActorRollResultUI result;

        for (int i = 0; i < data["rollsWithActors"].Count; i++)
        {
            int rollNumber = data["rollsWithActors"][i]["roll"].AsInt;
            string rollActorId = data["rollsWithActors"][i]["actorId"].Value;

            ActorData rollingActorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == rollActorId);
            if (rollingActorDat == null)
            {
                CORE.Instance.LogMessageError("No actor with ID " + data["rollsWithActors"][i]["actorId"].Value);
                continue;
            }


            result = ResourcesLoader.Instance.GetRecycledObject("ActorRollResultOnChar").GetComponent<ActorRollResultUI>();
            result.SetInfo(rollingActorDat.ActorEntity, rolledItem.Data, rollNumber);
        }

        // Telling who picked the item.
        string winningActorId = data["winningActorId"].Value;
        if (string.IsNullOrEmpty(winningActorId))
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No one has picked the item " + rolledItem.Data.DisplayName, Color.red, 3f));
            return;
        }


        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == winningActorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["winningActorId"].Value);
            return;
        }


        CORE.Instance.DelayedInvokation(2f, () =>
        {
            result = ResourcesLoader.Instance.GetRecycledObject("ActorRollResultOnCharWinner").GetComponent<ActorRollResultUI>();
            result.SetInfo(actorDat.ActorEntity, rolledItem.Data, 0);
            CORE.Instance.AddChatMessage("<color=yellow>" + actorDat.name + " has won the item: '" + rolledItem.itemName + "'</color>");
        });
    }

    public void OnActorChatMessage(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        if (actorDat.ActorEntity == null)
        {
            CORE.Instance.LogMessageError("No entity to actor with ID " + data["actorId"].Value);
            return;
        }

        actorDat.ActorEntity.ShowTextBubble(data["message"].Value);

        string chatlogMessage = "<color=yellow>" + actorDat.name + "</color>: " + data["message"].Value;

        CORE.Instance.AddChatMessage(chatlogMessage);
    }

    // Party

    public void SendPartyInvite(string actorName)
    {
        JSONNode node = new JSONClass();
        node["actorName"] = actorName;

        SendEvent("party_invite", node);
    }

    public void SendPartyInviteResponse(bool accept)
    {
        JSONNode node = new JSONClass();
        node["accept"].AsBool = accept;

        SendEvent("party_invite_response", node);
    }

    public void SendPartyLeave()
    {
        JSONNode node = new JSONClass();

        SendEvent("party_leave", node);
    }

    public void SendPartyKick(string actorName)
    {
        JSONNode node = new JSONClass();
        node["actorName"] = actorName;

        SendEvent("party_kick", node);
    }

    public void SendPartyLeader(string actorName)
    {
        JSONNode node = new JSONClass();
        node["actorName"] = actorName;

        SendEvent("party_leader", node);
    }

    public void OnPartyInvite(string eventName, JSONNode data)
    {
        string leaderName = data["leaderName"].Value;

        if (!string.IsNullOrEmpty(leaderName))
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + leaderName + " had invited you to a party!</color>");
            LootRollPanelUI.Instance.AddPartyInvitation(leaderName);
        }
        else
        {
            string actorName = data["actorName"].Value;
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has been invited to the party!</color>");
        }
    }

    public void OnPartyInviteTimeout(string eventName, JSONNode data)
    {
        CORE.Instance.AddChatMessage("<color=yellow> The party invitation had timed out...</color>");
        LootRollPanelUI.Instance.RemovePartyInvitation();
    }

    public void OnPartyJoin(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;

        CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has joined the party!</color>");
    }

    public void OnPartyDecline(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        string reason = data["reason"].Value;

        if (reason == "decline")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has declined the invitation.</color>");
        }
        else if (reason == "timeout")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + "'s invitation timed out.</color>");
        }
        else if (reason == "disconnected")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " was rude enough to disconnect.</color>");
        }
    }

    public void OnPartyLeave(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        string reason = data["reason"].Value;

        if (reason == "leave")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has left the party.</color>");
        }
        else if (reason == "kicked")
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " was kicked out of the party.</color>");
        }
    }

    public void OnPartyLeader(string eventName, JSONNode data)
    {
        string leaderName = data["leaderName"].Value;

        CORE.Instance.AddChatMessage("<color=yellow>" + leaderName + " is now the party leader!</color>");
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(leaderName + " is now the party leader!"));
    }

    public void OnPartyToggleOffline(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        bool isOffline = data["offline"].AsBool;

        if (isOffline)
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has gone offline</color>");
        }
        else
        {
            CORE.Instance.AddChatMessage("<color=yellow>" + actorName + " has come online!</color>");
        }
    }

    public void OnPartyStatus(string eventName, JSONNode data)
    {
        CORE.Instance.CurrentParty = JsonConvert.DeserializeObject<PartyData>(data["party"].ToString());

        List<ActorData> partyMembers = CORE.Instance.Room.Actors.FindAll(X => X.ActorEntity.InParty);
        partyMembers.ForEach(x => x.ActorEntity.InParty = false);

        if (CORE.Instance.CurrentParty != null)
        {
            foreach (string member in CORE.Instance.CurrentParty.members)
            {
                ActorData actor = CORE.Instance.Room.Actors.Find(x => x.name == member);
                if (actor == null)
                {
                    continue;
                }

                actor.ActorEntity.InParty = true;
            }
        }

        CORE.Instance.InvokeEvent("PartyUpdated");
    }


    // Expedition

    public void SendEnterExpedition(string expeditionName)
    {
        JSONNode node = new JSONClass();
        node["expeditionName"] = expeditionName;

        SendEvent("expedition_enter", node);
    }

    public void SendStartExpeditionQueue(string expeditionName)
    {
        JSONNode node = new JSONClass();
        node["expeditionName"] = expeditionName;

        SendEvent("expedition_queue_start", node);
    }

    public void SendStopExpeditionQueue()
    {
        JSONNode node = new JSONClass();

        SendEvent("expedition_queue_abort", node);
    }

    public void SendExpeditionQueueMatchResponse(bool accept)
    {
        JSONNode node = new JSONClass();
        node["accept"].AsBool = accept;

        SendEvent("expedition_queue_match_response", node);
    }


    public void OnExpeditionQueueStart(string eventName, JSONNode data)
    {
        string expeditionName = data["expeditionName"].Value;

        ExpeditionQueTimerUI.Instance.Show(expeditionName);
    }

    public void OnExpeditionQueueStop(string eventName, JSONNode data)
    {
        ExpeditionQueTimerUI.Instance.Hide();
    }

    public void OnExpeditionQueueMatchFound(string eventName, JSONNode data)
    {
        LootRollPanelUI.Instance.AddMatchFound();
        // TODO show loot roll window to accept / decline the queue. Keep the loot roll open until OnExpeditionQueueMatchHide is called~! (EVEN IF THE TIMER RUNS OUT!)
    }

    public void OnExpeditionQueueMatchHide(string eventName, JSONNode data)
    {
        LootRollPanelUI.Instance.RemoveMatchFound();
        // TODO hide the expedition queue match loot roll window
    }

    #endregion
}


[Serializable]
public class UserData
{
    public ActorData actor;

    public ActorData[] chars;

    public int SelectedCharacterIndex;
}

[Serializable]
public class ActorData
{
    public string actorId;
    public float x;
    public float y;
    public bool faceRight;
    public string name;
    public string classJob;
    public string actorType;
    public string prefab;
    public int hp;
    public ActorLooks looks = new ActorLooks();
    public List<Item> items;
    public int money;
    public int exp;
    public int level;
    public Dictionary<string, Item> equips = new Dictionary<string, Item>();
    public List<Item> orbs = new List<Item>();

    public bool isMob
    {
        get
        {
            return actorType == "mob";
        }
    }
    public bool isCharacter
    {
        get
        {
            return actorType == "player";
        }
    }
    public bool isBot
    {
        get
        {
            return actorType == "bot";
        }
    }
    public bool IsPlayer
    {
        get
        {
            return actorId == SocketHandler.Instance.CurrentUser.actor.actorId;
        }
    }

    public AttributeData attributes;

    public Dictionary<string, StateData> states = new Dictionary<string, StateData>();

    public List<string> abilities;

    [JsonIgnore]
    public float MovementSpeed
    {
        get
        {
            return CORE.Instance.Data.content.BaseAttributes.MovementSpeed + CORE.Instance.Data.content.BaseAttributes.MovementSpeed * attributes.MovementSpeed;
        }
    }

    [JsonIgnore]
    public float MaxHP
    {
        get
        {
            return CORE.Instance.Data.content.BaseAttributes.HP + CORE.Instance.Data.content.BaseAttributes.HP * attributes.HP;
        }
    }

    [JsonIgnore]
    public Actor ActorEntity;

    [JsonIgnore]
    public ClassJob ClassJobReference
    {
        get
        {
            if (_classjobRef == null && !string.IsNullOrEmpty(classJob))
            {
                _classjobRef = CORE.Instance.Data.content.Classes.Find(x => x.name == this.classJob);
            }

            return _classjobRef;
        }
        set
        {
            _classjobRef = value;
        }
    }
    ClassJob _classjobRef;


    [JsonIgnore]
    public UnityEvent OnRefreshStates = new UnityEvent();

    [JsonIgnore]
    public UnityEvent OnRefreshAbilities = new UnityEvent();

    public ActorData(string gName, string gClassJob, GameObject gActorObject = null)
    {
        this.name = gName;
        this.classJob = gClassJob;

        if (gActorObject != null)
        {
            this.ActorEntity = gActorObject.GetComponent<Actor>();
        }
    }
}

[Serializable]
public class ActorLooks
{
    public bool IsFemale;
    public string Hair;
    public string Eyebrows;
    public string Eyes;
    public string Ears;
    public string Nose;
    public string Mouth;

    public string SkinColor;
    public string HairColor;
    public string Iris;
}

[Serializable]
public class Interactable
{
    public float x;
    public float y;
    public string interactableId;
    public string interactableName;
    public string actorId;

    [JsonIgnore]
    public InteractableData Data
    {
        get
        {
            if (_data == null)
            {
                _data = CORE.Instance.Data.content.Interactables.Find(x => x.name == interactableName);
            }

            return _data;
        }
        set
        {
            _data = value;
        }
    }
    [JsonIgnore]
    InteractableData _data;

    [JsonIgnore]
    public InteractableEntity Entity;
}

public class SocketEventListener
{
    public string EventKey;

    public Action<string, JSONNode> InternalCallback;




    public SocketEventListener(string key, Action<string, JSONNode> internalCallback = null)
    {
        this.EventKey = key;

        this.InternalCallback = internalCallback;
    }

    public void Callback(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data = JSON.Parse(args[0].ToString());
        InternalCallback.Invoke(packet.EventName, data);
    }
}

