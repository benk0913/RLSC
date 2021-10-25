using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using EdgeworldBase;
using Newtonsoft.Json;
using PlatformSupport.Collections.ObjectModel;
using SimpleJSON;
using Steamworks;
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
    
    public ServerEnvironment ServerEnvironment;

    public UserData CurrentUser;

    public int SelectedRealmIndex = -1;

    public SocketManager SocketManager;

    public List<SocketEventListener> SocketEventListeners = new List<SocketEventListener>();

    public int UniqueNumber = 0;


    //STEAM
    int SessionTicketSize = 1024;
    byte[] SessionPTicket;
    uint SessionPCBTicket = 0;

    public string SessionTicket;

    public string TutorialIndex;

#if DEV_BUILD || UNITY_EDITOR
    public bool RandomUser;
#endif

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
        SocketManager = new SocketManager(new Uri(ServerEnvironment.HostUrl));

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

        // Cash Items
        SocketEventListeners.Add(new SocketEventListener("cash_refresh", OnCashRefresh));
        SocketEventListeners.Add(new SocketEventListener("actor_change_looks", OnActorChangeLooks));
        SocketEventListeners.Add(new SocketEventListener("user_char_slots", OnUserCharSlots));
        SocketEventListeners.Add(new SocketEventListener("actor_change_name_prompt", OnActorChangeNamePrompt));
        SocketEventListeners.Add(new SocketEventListener("actor_change_name", OnActorChangeName));

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

        //Karma
        SocketEventListeners.Add(new SocketEventListener("karma_update", OnKarmaUpdate));

        //Trade
        SocketEventListeners.Add(new SocketEventListener("trade_state_updated", OnTradeStateUpdated));
        SocketEventListeners.Add(new SocketEventListener("trade_complete", OnTradeComplete));
        SocketEventListeners.Add(new SocketEventListener("player_stopped_trade", OnTradeStopped));
        SocketEventListeners.Add(new SocketEventListener("player_accept_trade", OnPlayerAccept));
        SocketEventListeners.Add(new SocketEventListener("player_dont_accept_trade", OnPlayerDontAccept));

        //Friends
        SocketEventListeners.Add(new SocketEventListener("friends_list_update", OnFriendsListUpdated));

        //Que
        SocketEventListeners.Add(new SocketEventListener("que_update", OnQueUpdate));
        SocketEventListeners.Add(new SocketEventListener("que_complete", OnQueComplete));

        //Emotes
        SocketEventListeners.Add(new SocketEventListener("actor_emoted", OnActorEmoted));

        foreach (SocketEventListener listener in SocketEventListeners)
        {
            listener.InternalCallback = AddEventListenerLogging + listener.InternalCallback;
        }
    }

    void AddEventListenerLogging(string eventName, JSONNode data)
    {
        if (!CORE.Instance.DEBUG || (!CORE.Instance.DEBUG_SPAMMY_EVENTS && (!string.IsNullOrEmpty(data["actorPositions"].ToString()) || !string.IsNullOrEmpty(data["actorMoved"].ToString()))))
        {
            return;
        }

        CORE.Instance.LogMessage("On Socket Event: " + eventName + " | " + FormatJson(data.ToString()));
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

    public void GetSteamSession(Action OnComplete = null)
    {
#if DEV_BUILD || UNITY_EDITOR
        if (RandomUser) {
            OnComplete?.Invoke();
            return;
        }
#endif
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Waiting for steam to initialize...", Colors.AsColor(Colors.COLOR_GOOD), 3f, false));

        if(GetSessionTicketRoutineInstance != null)
        {
            StopCoroutine(GetSessionTicketRoutineInstance);
        }

        GetSessionTicketRoutineInstance = StartCoroutine(GetSessionTicketRoutine(OnComplete));
    }


    Coroutine GetSessionTicketRoutineInstance;
    IEnumerator GetSessionTicketRoutine(Action OnComplete = null)
    {
        float timeout = 10f;
        while(timeout > 0)
        {
            if(SteamManager.Initialized)
            {
                break;
            }

            timeout -= Time.deltaTime;

            if(timeout <= 0)
            {
                timeout = 10f;
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Please make sure Steam is running and online!", Colors.AsColor(Colors.COLOR_BAD), 3f, true));

                ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);

                WarningWindowUI.Instance.Show("Steam is unavailable. Retry?", () => 
                {
                    ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

                    GetSteamSession(OnComplete);
                },false, () => 
                {
                    Application.Quit();
                },"Steam Unavailable");

                yield break;
            }

            yield return 0;
        }

        ObtainSessionTicket(OnComplete);
    }

    Coroutine currentTimeoutValidation;
    void ObtainSessionTicket(Action OnComplete)
    {
        this.OnCompleteSessionTicket = OnComplete;
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Getting Session...", Colors.AsColor(Colors.COLOR_GOOD), 3f, true));
        
        if(GetAuthSessionTicketResponseCallbackContainer == null)
            GetAuthSessionTicketResponseCallbackContainer = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);

        if(GetAuthSessionTicketOnCompleteCallbackContainer == null)
            GetAuthSessionTicketOnCompleteCallbackContainer = Callback<GetAuthSessionTicketResponse_t>.Create((GetAuthSessionTicketResponse_t pc)=>{ OnCompleteSessionTicket?.Invoke();});

        
        SessionPTicket = new byte[SessionTicketSize];
        SteamUser.GetAuthSessionTicket(SessionPTicket,SessionTicketSize, out SessionPCBTicket);

        //Timeout validation

        if(currentTimeoutValidation != null)
        {
            StopCoroutine(currentTimeoutValidation);
        }

        currentTimeoutValidation = CORE.Instance.DelayedInvokation(10f, () => 
        {
            if(string.IsNullOrEmpty(SessionTicket))
            {
                GetSteamSession(OnComplete);
            }
        });
        

    }


    Action OnCompleteSessionTicket;
    protected Callback<GetAuthSessionTicketResponse_t> GetAuthSessionTicketOnCompleteCallbackContainer;
    protected Callback<GetAuthSessionTicketResponse_t> GetAuthSessionTicketResponseCallbackContainer;
    void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback) 
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Connecting", Colors.AsColor(Colors.COLOR_GOOD), 3f, true));
        //System.Array.Resize(ref SessionPTicket, (int)SessionPCBTicket);
            

        string hexticket ="";
        for(int i=0;i<SessionPTicket.Length;i++) {
            hexticket += string.Format("{0:x2}", SessionPTicket[i]);
        }

        this.SessionTicket = hexticket;
        CORE.Instance.LogMessage("Current session: "+this.SessionTicket);
        
    }

    public void LogOut()
    {

        CurrentUser.chars = null;
        CurrentUser.info = null;
        TutorialIndex = "";
        SessionTicket = "";
        ServerEnvironment.Region = "";

        if(currentTimeoutValidation != null)
        {
            StopCoroutine(currentTimeoutValidation);
            currentTimeoutValidation = null;
        }

        CORE.Instance.LoadScene("MIDLOADER",()=> 
        {
            if (CORE.Instance.InGame)
                SendDisconnectSocket();
            else
                Disconnect();
        });
    }

    public void SendLogin(Action OnComplete)
    {
        JSONNode node = new JSONClass();
        
        node["skipTutorial"] = SessionTicket;
        node["tutorialVersion"] = Application.version;
        
#if DEV_BUILD || UNITY_EDITOR
    if (RandomUser) {
    #if DEV_BUILD
        node["randomUser"] = "dev";
    #elif UNITY_EDITOR
        node["randomUser"] = "editor";
    #endif
    }
#endif

        Action loginAction = () =>
        {
            Dictionary<string, string> UrlParams = new Dictionary<string, string>();
            UrlParams.Add("realm", SelectedRealmIndex.ToString());

            SendWebRequest(ServerEnvironment.HostUrl + "/login", (UnityWebRequest lreq) =>
            {
                OnLogin(lreq);

                CORE.Instance.DelayedInvokation(0.2f, () =>
                    {
                        OnComplete?.Invoke();
                    });
            },
            node.ToString(),
            UrlParams,
            true,
            (UnityWebRequest lreq) => 
            {
                ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);

                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Login FAILED! - " + lreq.result.ToString(), Color.red, 3f, true));
                WarningWindowUI.Instance.Show("Failed to login. Retry?", () =>
                {
                    ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

                    SendLogin(OnComplete);
                }, false, () =>
                {
                    Application.Quit();
                },"Login Error");
            });
        };

        if (SelectedRealmIndex == -1)
        {
            RealmSelectionUI.Instance.Show((int selectedRealm) => 
            {
                SelectedRealmIndex = selectedRealm;
                loginAction.Invoke();
            });
        }
        else
        {
            loginAction.Invoke();
        }
    }

    public void SendCreateCharacter(string element = "fire", ActorData actor = null, Action OnComplete = null, Action OnError = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Trying to create character...", Colors.AsColor(Colors.COLOR_GOOD), 1f, true));

        JSONNode node = new JSONClass();
        node["tutorialIndex"] = TutorialIndex;
        node["classJob"] = element;
        node["actor"] = JSON.Parse(JsonConvert.SerializeObject(actor));
        
        Dictionary<string, string> UrlParams = new Dictionary<string, string>();
        UrlParams.Add("realm", SelectedRealmIndex.ToString());

        SendWebRequest(ServerEnvironment.HostUrl + "/create-char", (UnityWebRequest ccreq) =>
        {
            OnCreateCharacter(ccreq);


            OnComplete?.Invoke();
        },
        node.ToString(),
        UrlParams,
        true,
        (UnityWebRequest err)=>
        {
            CORE.Instance.LogMessageError(err.error);
            OnError?.Invoke();
        });
    }

    public void SendGetRandomName(bool IsFemale, Action<string> OnComplete)
    {
        JSONNode node = new JSONClass();
        node["tutorialIndex"] = TutorialIndex;
        node["isFemale"].AsBool = IsFemale;
        
        Dictionary<string, string> UrlParams = new Dictionary<string, string>();
        UrlParams.Add("realm", SelectedRealmIndex.ToString());

        SendWebRequest(ServerEnvironment.HostUrl + "/random-name", (UnityWebRequest ccreq) =>
        {
            JSONNode data = JSON.Parse(ccreq.downloadHandler.text);

            OnComplete.Invoke(data["name"]);
        },
        node.ToString(),
        UrlParams,
        true);
    }

    public void SendDeleteCharacter(string actorId, Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Deleting Character...", Colors.AsColor(Colors.COLOR_GOOD), 1f, true));

        JSONNode node = new JSONClass();
        node["tutorialIndex"] = TutorialIndex;
        node["actorId"] = actorId;
        
        Dictionary<string, string> UrlParams = new Dictionary<string, string>();
        UrlParams.Add("realm", SelectedRealmIndex.ToString());

        SendWebRequest(ServerEnvironment.HostUrl + "/delete-char", (UnityWebRequest ccreq) =>
        {
            OnDeleteCharacter(ccreq);


            OnComplete?.Invoke();
        },
        node.ToString(),
        UrlParams,
        true);
    }

    public void SendSelectCharacter(Action OnComplete = null, int index = 0)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Selecting Character", Colors.AsColor(Colors.COLOR_GOOD), 1f, true));

        CurrentUser.SelectedCharacterIndex = index;
        CurrentUser.actor = CurrentUser.chars[index];

        SendConnectSocket(OnComplete);
    }

    public void SendConnectSocket(Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Connecting...", Colors.AsColor(Colors.COLOR_GOOD), 3f, true));

        if (ConnectSocketRoutineInstance != null)
        {
            StopCoroutine(ConnectSocketRoutineInstance);
        }
    
        ConnectSocketRoutineInstance = StartCoroutine(ConnectSocketRoutine(OnComplete));
    }

    public void SendGeolocationRequest(Action<UnityWebRequest> OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Looking for a nearby server...", Colors.AsColor(Colors.COLOR_GOOD), 3f, true));

        SendWebRequest(ServerEnvironment.HostUrl + "/which-region", (UnityWebRequest ccreq) =>
        {
            OnComplete?.Invoke(ccreq);
        },
        null,
        null,
        false);
    }

    public void SendRealmCapacityRequest(int realmIndex, Action<UnityWebRequest> OnComplete = null)
    {
        Dictionary<string, string> UrlParams = new Dictionary<string, string>();
        UrlParams.Add("realm", realmIndex.ToString());

        SendWebRequest(ServerEnvironment.HostUrl + "/capacity", (UnityWebRequest ccreq) =>
        {
            OnComplete?.Invoke(ccreq);
        },
        null,
        UrlParams,
        false);
    }
    

    Coroutine ConnectSocketRoutineInstance;

    IEnumerator ConnectSocketRoutine(Action OnComplete = null)
    {
        //BestHTTP.HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.All; //Uncomment to log socket...

        SocketOptions options = new SocketOptions();
        options.AdditionalQueryParams = new ObservableDictionary<string, string>();
        options.AdditionalQueryParams.Add("tutorialIndex", TutorialIndex);
        options.AdditionalQueryParams.Add("tutorialVersion", Application.version);
        options.AdditionalQueryParams.Add("charIndex", CurrentUser.SelectedCharacterIndex.ToString());
        options.AdditionalQueryParams.Add("realm", SelectedRealmIndex.ToString());

#if UNITY_EDITOR
        options.AdditionalQueryParams.Add("isEditor", "1");
#endif

        options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
        options.AutoConnect = false;

        DisconnectSocket();

        SocketManager = new SocketManager(new Uri(ServerEnvironment.SocketUrl), options);
        SocketManager.Encoder = new SimpleJsonEncoder();

        AddListeners();

        SocketManager.Open();

        float timeout = 10f;
        while (SocketManager.State != SocketManager.States.Open)
        {
            timeout -= Time.deltaTime;
            yield return 0;

            if(timeout <= 0f)
            {
                break;
            }
        }

        if(SocketManager.State != SocketManager.States.Open)
        {
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Realm Temporarily Closed!", Colors.AsColor(Colors.COLOR_BAD), 3f, true));
            ResourcesLoader.Instance.LoadingWindowObject.SetActive(false);


            WarningWindowUI.Instance.Show("Unable to enter the selected realm. Retry?", () =>
            {
                ResourcesLoader.Instance.LoadingWindowObject.SetActive(true);

                SendConnectSocket(OnComplete);
            }, false, () =>
            {
                Application.Quit();
            },"Realm Unavailable");

            yield break;
        }

        OnComplete?.Invoke();

        CORE.Instance.LogMessage("Connected To Socket.");

        ConnectSocketRoutineInstance = null;
    }

    public void SendDisconnectSocket()
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Disconnecting...", Colors.AsColor(Colors.COLOR_GOOD), 3f, true));

        DisconnectSocket();

        CORE.Instance.LogMessage("Disconnected From Socket.");
    }

    private void DisconnectSocket()
    {
        if(CORE.Instance != null)
        {
            CORE.Instance.DisposeSession();
        }

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

        CORE.Instance.DelayedInvokation(0.1f,()=>
        {
            CurrentUser.chars = JsonConvert.DeserializeObject<List<ActorData>>(data["chars"].ToString());
            CurrentUser.info = JsonConvert.DeserializeObject<UserInfo>(data["info"].ToString());
            TutorialIndex = data["tutorialIndex"].Value;
        });
    }

    public void OnCreateCharacter(UnityWebRequest response)
    {
        JSONNode data = JSON.Parse(response.downloadHandler.text);

        ActorData actor = JsonConvert.DeserializeObject<ActorData>(data["actor"].ToString());
        CurrentUser.chars.Insert(0, actor);
    }
    public void OnDeleteCharacter(UnityWebRequest response)
    {
        // TODO update characters list
    }


    #endregion

    #region HTTP Request Handling

    public void SendWebRequest(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null, bool isPost = false, Action<UnityWebRequest> OnError = null)
    {
        StartCoroutine(SendHTTPRequestRoutine(url, OnResponse, sentJson, urlParams, isPost, OnError));
    }

    public IEnumerator SendHTTPRequestRoutine(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null, bool isPost = false, Action<UnityWebRequest> OnError = null)
    {
        if(string.IsNullOrEmpty(sentJson))
        {
            sentJson = new JSONClass();
        }
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
                CORE.Instance.LogMessage("Request: " + urlWithParams + " | " + FormatJson(sentJson));
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
                CORE.Instance.LogMessage("Request: " + url + " | " + FormatJson(sentJson));
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

        if (request.result == UnityWebRequest.Result.ConnectionError 
        || request.result == UnityWebRequest.Result.ProtocolError 
        || request.result == UnityWebRequest.Result.DataProcessingError)
        {
            OnError?.Invoke(request);
            CORE.Instance.LogMessageError(request.error + " | " + request.downloadHandler.text);
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(request.downloadHandler.text, Colors.AsColor(Colors.COLOR_BAD), 2f, true));

            yield break;
        }


        if (CORE.Instance.DEBUG)
        {
            if(CORE.Instance.DEBUG_SPAMMY_EVENTS)
            {
                CORE.Instance.LogMessage(url+" | " + request.downloadHandler.text);
            }

            try
            {
                CORE.Instance.LogMessage("Response: " + url + " | " + FormatJson(request.downloadHandler.text));
            }
            catch{}
        }



        OnResponse?.Invoke(request);

    }

    public static string FormatJson(string json)
    {
        object parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
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

        if (CORE.Instance.DEBUG && (eventKey != "actors_moved" || CORE.Instance.DEBUG_SPAMMY_EVENTS))
        {
            CORE.Instance.LogMessage("Sending Event: " + eventKey + " | " + FormatJson(node.ToString()));
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
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(errorMessage, Colors.AsColor(Colors.COLOR_BAD), 2, true));
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
        Disconnect();
    }

    void Disconnect()
    {
        CORE.Instance.InvokeEvent("Disconnect");
        CORE.Instance.LogMessage("DISCONNECTING");

        if (CORE.Instance != null)
        {
            CORE.Instance.DisposeSession();
        }

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

        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(error, Colors.AsColor(Colors.COLOR_BAD), durationInSeconds, true));
        CORE.Instance.LogMessageError(error);
    }

    public void OnLoadScene(string eventName, JSONNode data)
    {
        // TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Entering " + data["scene"].Value, Colors.AsColor(Colors.COLOR_GOOD), 1f, false));

        CORE.Instance.IsLoading = true;
        CORE.Instance.CloseCurrentWindow();
        ScreenFaderUI.Instance.FadeToBlack(() =>
        {
            float newSceneFocusCameraX = data["x"].AsFloat;
            float newSceneFocusCameraY = data["y"].AsFloat;

            CORE.Instance.LoadScene(data["scene"].Value, () =>
            {
                CORE.Instance.UpdateSteamStatus();

                SendEvent("scene_loaded");
                CORE.Instance.IsLoading = false;

                SceneInfo sceneInfo = CORE.Instance.ActiveSceneInfo;
                CORE.Instance.NextScenePrediction = (string)data["nextScenePrediction"];
                CORE.Instance.InvokeEvent("PredictionUpdate");

                if (CameraChaseEntity.Instance != null && CameraChaseEntity.Instance.Speed > 0)
                {
                    CameraChaseEntity.Instance.transform.position = new Vector3(newSceneFocusCameraX, newSceneFocusCameraY, Camera.main.transform.position.z);
                }

                CORE.Instance.RefreshSceneInfo();

                if (sceneInfo.displayTitleOnEnter)
                {
                    CORE.Instance.ShowScreenEffect("ScreenEffectLocation", sceneInfo.displyName);

                }
                
                ScreenFaderUI.Instance.FadeFromBlack();
                    
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

    public void OnKarmaUpdate(string eventName, JSONNode data)
    {
        CurrentUser.actor.karma = data["karma"].AsInt;
        CORE.Instance.InvokeEvent("AlignmentUpdated");
    }

    public void OnTradeStateUpdated(string eventName, JSONNode data)
    {
        TradeWindowUI.PlayerTradeState player = JsonConvert.DeserializeObject<TradeWindowUI.PlayerTradeState>(data["player"].ToString()); 
        TradeWindowUI.PlayerTradeState otherPlayer = JsonConvert.DeserializeObject<TradeWindowUI.PlayerTradeState>(data["otherPlayer"].ToString()); 
        TradeWindowUI.Instance.TradeStateUpdated(player,otherPlayer);
    }

    public void OnTradeComplete(string eventName, JSONNode data)
    {
        TradeWindowUI.PlayerTradeState player = JsonConvert.DeserializeObject<TradeWindowUI.PlayerTradeState>(data["player"].ToString()); 
        TradeWindowUI.PlayerTradeState otherPlayer = JsonConvert.DeserializeObject<TradeWindowUI.PlayerTradeState>(data["otherPlayer"].ToString()); 
        TradeWindowUI.Instance.TradeComplete(player,otherPlayer);
    }
    
    public void OnTradeStopped(string eventName, JSONNode data)
    {
       string byWho = data["playerId"].Value;
       TradeWindowUI.Instance.StopTrade(byWho);
    }

    public void OnPlayerAccept(string eventName, JSONNode data)
    {
        string byWho = data["playerId"].Value;
        TradeWindowUI.Instance.AcceptTrade(byWho);
    }

    public void OnPlayerDontAccept(string eventName, JSONNode data)
    {
        string byWho = data["playerId"].Value;
        TradeWindowUI.Instance.DontAcceptTrade(byWho);
    }

    public void OnFriendsListUpdated(string eventName, JSONNode data)
    {
        CurrentUser.friends = JsonConvert.DeserializeObject<UserData.FriendData[]>(data["friends"].ToString());
        CORE.Instance.InvokeEvent("FriendsUpdated");
    }

    public void OnQueUpdate(string eventName, JSONNode data)
    {
        QueWindowUI.Instance.Show(data["players_before"].AsInt);
    }

    public void OnQueComplete(string eventName, JSONNode data)
    {
        QueWindowUI.Instance.Hide();
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

    public void SendDroppedEquip(string equipType)
    {
        JSONNode node = new JSONClass();
        node["equipType"] = equipType;

        SocketHandler.Instance.SendEvent("dropped_equip", node);
    }

    public void SendUsedItem(int slotIndex, bool isCash)
    {
        JSONNode node = new JSONClass();
        node["slotIndex"].AsInt = slotIndex;
        node["isCash"].AsBool = isCash;

        SocketHandler.Instance.SendEvent("used_item", node);
    }

    public void SendUnequippedItem(string equipType)
    {
        JSONNode node = new JSONClass();
        node["equipType"] = equipType;

        SocketHandler.Instance.SendEvent("unequipped_item", node);
    }

    public void SendSwappedItemSlots(int slotIndex1, int slotIndex2, bool isCash)
    {
        JSONNode node = new JSONClass();
        node["slotIndex1"].AsInt = slotIndex1;
        node["slotIndex2"].AsInt = slotIndex2;
        node["isCash"].AsBool = isCash;

        SocketHandler.Instance.SendEvent("swapped_item_slots", node);
    }

    public void SendSwappedItemAndEquipSlots(int slotIndex, string equipType, bool isCash)
    {
        JSONNode node = new JSONClass();
        node["equipType"] = equipType;
        node["slotIndex"].AsInt = slotIndex;
        node["isCash"].AsBool = isCash;

        SocketHandler.Instance.SendEvent("swapped_item_and_equip_slots", node);
    }

    public void SendSwappedEquipAndEquipSlots(string equipType1, string equipType2)
    {
        JSONNode node = new JSONClass();
        node["equipType1"] = equipType1;
        node["equipType2"] = equipType2;

        SocketHandler.Instance.SendEvent("swapped_equip_and_equip_slots", node);
    }

    public void OnMoveActors(string eventName, JSONNode data)
    {
        CORE.Instance.Room.ReceiveActorPositions(data);
    }

    protected void OnBitchPlease(string eventName, JSONNode data)
    {
        if(CORE.Instance.NEVER_BITCH)
        {
            return;
        }

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
        bool castingExternal = data["castingExternal"].AsBool;
        // TODO run the ability if possible when castingExternal is true.


        if(castingExternal)
        {
            int index = actorDat.ClassJobReference.Abilities.FindIndex(x=> x==abilityName);
            actorDat.ActorEntity.AttemptPrepareAbility(index);
        }
        else
        {
            if (!actorDat.ActorEntity.IsClientControl)
            {
                actorDat.ActorEntity.PrepareAbility(CORE.Instance.Data.content.Abilities.Find(x => x.name == abilityName));
            }   
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
        actorDat.computedAttributes = JsonConvert.DeserializeObject<AttributeData>(data["computedAttributes"].ToString());
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

    public void OnActorEmoted(string eventName, JSONNode data)
    {
        string givenActorId = data["actorId"].Value;
        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == givenActorId);

        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        Emote emote = CORE.Instance.Data.content.Emotes.Find(x=>x.name ==  data["emote"].Value);

        if(emote == null)
        {
            CORE.Instance.LogMessageError("No emote with name: " + data["emote"].Value);
            return;
        }

        if(actorDat.ActorEntity == null)
        {
            CORE.Instance.LogMessageError("No actor entity for actorID " + data["actorId"].Value);
            return;
        }

        actorDat.ActorEntity.Emote(emote);
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

        bool isCash = data["isCash"].AsBool;
        if (isCash)
        {
            CurrentUser.info.cashItems[slotIndex] = item;
        }
        else
        {
            CurrentUser.actor.items[slotIndex] = item;
        }

        CORE.Instance.InvokeEvent("InventoryUpdated");
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
            InventoryUI.Instance.EquippedItem(item);
            CORE.Instance.InvokeEvent("InventoryUpdated");
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

        string hasPicedUp = " has picked up ";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation("% chance to ", out hasPicedUp);

        string coins = "coins";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation("coins", out coins);

        string hasPicedUpItem = " has picked up the item: '";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has picked up the item: '", out hasPicedUpItem);


        item.Entity.BePickedBy(actorDat.ActorEntity);
        if (item.Data.Type.name == "Money")
        {
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorDat.name + hasPicedUp + String.Format("{0:n0}", item.amount) + " "+coins+"</color>");
        }
        else
        {
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorDat.name + hasPicedUpItem + item.itemName + "'</color>");
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

        CORE.Instance.InvokeEvent("InventoryUpdated");
    }

    public void OnCashRefresh(string eventName, JSONNode data)
    {
        int cash = data["cash"].AsInt;

        SocketHandler.Instance.CurrentUser.info.cashPoints = cash;

        CORE.Instance.InvokeEvent("InventoryUpdated");
    }

    public void OnUserCharSlots(string eventName, JSONNode data)
    {
        int additionalCharSlots = data["additionalCharSlots"].AsInt;

        SocketHandler.Instance.CurrentUser.info.additionalCharSlots = additionalCharSlots;
    }

    public void OnActorChangeLooks(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }
        ActorLooks newActorLooks = JsonConvert.DeserializeObject<ActorLooks>(data["looks"].ToString());

        actorDat.looks = newActorLooks;
        actorDat.ActorEntity.RefreshLooks();
    }

    public void OnActorChangeNamePrompt(string eventName, JSONNode data)
    {
        InputLabelWindow.Instance.Show("Change Name", "New name", (string newName) => 
        {
            JSONClass node = new JSONClass();

            node["name"] = newName;
            
            SocketHandler.Instance.SendEvent("changed_name",node);
        });
    }

    public void OnActorChangeName(string eventName, JSONNode data)
    {
        string actorId = data["actorId"].Value;

        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == actorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }
        string newName = data["name"].Value;
        
        actorDat.name = newName;
        actorDat.ActorEntity.RefreshName();
        CORE.Instance.InvokeEvent("StatsChanged");
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
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("No one has picked the item " + rolledItem.Data.DisplayName, Colors.AsColor(Colors.COLOR_BAD), 3f));
            return;
        }


        ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == winningActorId);
        if (actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["winningActorId"].Value);
            return;
        }

        string hasWonItem = " has won the item: '";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has won the item: '", out hasWonItem);

        string itemName = rolledItem.itemName;
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(rolledItem.itemName, out itemName);

        CORE.Instance.DelayedInvokation(2f, () =>
        {
            result = ResourcesLoader.Instance.GetRecycledObject("ActorRollResultOnCharWinner").GetComponent<ActorRollResultUI>();
            result.SetInfo(actorDat.ActorEntity, rolledItem.Data, 0);
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorDat.name + hasWonItem + itemName + "'</color>");
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

        string chatlogMessage = "<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorDat.name + "</color>: " + data["message"].Value;

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

        AudioControl.Instance.Play("getPartyInvite");

        if (!string.IsNullOrEmpty(leaderName))
        {
            string hasInvited = " had invited you to a party!";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" had invited you to a party!", out hasInvited);

            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + leaderName + hasInvited+"</color>");
            LootRollPanelUI.Instance.AddPartyInvitation(leaderName);
        }
        else
        {
            string hadInvitedYou = " has been invited to the party!";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has been invited to the party!", out hadInvitedYou);

            string actorName = data["actorName"].Value;
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + "</color>");
        }
    }

    public void OnPartyInviteTimeout(string eventName, JSONNode data)
    {
        string hadInvitedYou = "The party invitation had timed out...";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation("The party invitation had timed out...", out hadInvitedYou);

        CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + "> "+ hadInvitedYou + "</color>");
        LootRollPanelUI.Instance.RemovePartyInvitation();

        AudioControl.Instance.Play("getPartyTimeout");
    }

    public void OnPartyJoin(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;

        string hadInvitedYou = " has joined the party!";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has joined the party!", out hadInvitedYou);

        CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + "</color>");

        AudioControl.Instance.Play("getPartyAccept");

        
    }

    public void OnPartyDecline(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        string reason = data["reason"].Value;

        if (reason == "decline")
        {
            string translatedPart = " has declined the invitation.";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has declined the invitation.", out translatedPart);

            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + translatedPart+"</color>");
            AudioControl.Instance.Play("getPartyDecline");
        }
        else if (reason == "timeout")
        {
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + "'s invitation timed out.</color>");
            AudioControl.Instance.Play("getPartyTimeout");
        }
        else if (reason == "disconnected")
        {
            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + " was rude enough to disconnect.</color>");
        }
    }

    public void OnPartyLeave(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        string reason = data["reason"].Value;

        if (reason == "leave")
        {
            string translatedPart = " has left the party.";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has left the party.", out translatedPart);

            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + translatedPart+"</color>");
            AudioControl.Instance.Play("getPartyLeave");   
        }
        else if (reason == "kicked")
        {
            string translatedPart = " was kicked out of the party.";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" was kicked out of the party.", out translatedPart);

            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + translatedPart+"</color>");
            AudioControl.Instance.Play("getPartyKick");
        }
    }

    public void OnPartyLeader(string eventName, JSONNode data)
    {
        string leaderName = data["leaderName"].Value;

        string translatedPart = " is now the party leader!";
        CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" is now the party leader!", out translatedPart);

        CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + leaderName + translatedPart+"</color>");
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance(leaderName + translatedPart));

        AudioControl.Instance.Play("getPartyPromote");
    }

    public void OnPartyToggleOffline(string eventName, JSONNode data)
    {
        string actorName = data["actorName"].Value;
        bool isOffline = data["offline"].AsBool;

        if (isOffline)
        {
            string translatedPart = " has gone offline";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has gone offline", out translatedPart);


            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + translatedPart+"</color>");
        }
        else
        {
            string translatedPart = " has come online!";
            CORE.Instance.Data.Localizator.mSource.TryGetTranslation(" has come online!", out translatedPart);


            CORE.Instance.AddChatMessage("<color=" + Colors.COLOR_HIGHLIGHT + ">" + actorName + translatedPart +"</color>");
        }
    }

    public void OnPartyStatus(string eventName, JSONNode data)
    {
        PartyData newPartyData = JsonConvert.DeserializeObject<PartyData>(data["party"].ToString());

        if(CORE.Instance.CurrentParty == null && newPartyData != null) // Entering a party for hte first time 
        {
            if(newPartyData.leaderName == CORE.PlayerActor.name)//Leader of the new party
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, CORE.Instance.Data.content.MaxPartyMembers);
            }
            else if(newPartyData.steamLobbyId != default) //Not leader and steamid exists
            {
                SteamMatchmaking.JoinLobby(new CSteamID(newPartyData.steamLobbyId));
            }
        }
        else if (CORE.Instance.CurrentParty != null && newPartyData == null) //Leaving a party
        {
            SteamMatchmaking.LeaveLobby(new CSteamID(CORE.Instance.CurrentParty.steamLobbyId));
        }
        else if(CORE.Instance.CurrentParty != null && newPartyData != null)
        {
            if(CORE.Instance.CurrentParty.steamLobbyId != newPartyData.steamLobbyId) //Still in a party, different steampartyid
            {
                SteamMatchmaking.LeaveLobby(new CSteamID(CORE.Instance.CurrentParty.steamLobbyId));
                SteamMatchmaking.JoinLobby(new CSteamID(newPartyData.steamLobbyId));
            }
        }

        CORE.Instance.CurrentParty = newPartyData;



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

        CORE.Instance.InvokeEvent("MatchQueueUpdate");
    }

    public void OnExpeditionQueueStop(string eventName, JSONNode data)
    {
        ExpeditionQueTimerUI.Instance.Hide();

        CORE.Instance.InvokeEvent("MatchQueueUpdate");
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

    public List<ActorData> chars;

    public UserInfo info;

    public FriendData[] friends;

    public int SelectedCharacterIndex;

    [Serializable]
    public class FriendData
    {
        public string name;
        public bool isOnline;
    }
}

[Serializable]
public class UserInfo
{
    public List<Item> cashItems;

    public int cashPoints;

    public int additionalCharSlots;
}

[Serializable]
public class ActorData
{
    public string actorId;
    public float x;
    public float y;
    public int movementDirection;
    public bool faceRight;
    public string name;
    public string classJob;
    public ulong steamID;

    public bool alignmentGood;
    public int karma;
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

    public AttributeData computedAttributes = new AttributeData();

    public Dictionary<string, StateData> states = new Dictionary<string, StateData>();

    public List<string> abilities;

    [JsonIgnore]
    public float MovementSpeed
    {
        get
        {
            return computedAttributes.MovementSpeed;
        }
    }

    [JsonIgnore]
    public float MaxHP
    {
        get
        {
            return computedAttributes.HP;
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

    public ActorData Clone()
    {
        ActorData clone = (ActorData) this.MemberwiseClone();  
        clone.actorId = "DISPLAY_CHARACTER"; 
        clone.equips = new Dictionary<string, Item>();

        for(int i=0;i<equips.Keys.Count;i++)
        {
            Item item = (Item) this.equips[this.equips.Keys.ElementAt(i)];

            if(item != null)
            {
                clone.equips.Add(this.equips.Keys.ElementAt(i), item.Clone());
            }
            else
            {
                clone.equips.Add(this.equips.Keys.ElementAt(i), null);
            }
        }

        clone.looks = this.looks.Clone();
        
        return clone;
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

    public ActorLooks Clone() => (ActorLooks) MemberwiseClone();
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

