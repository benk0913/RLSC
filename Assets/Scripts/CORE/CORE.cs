using EdgeworldBase;
using Newtonsoft.Json;
using SimpleJSON;
#if !UNITY_ANDROID && !UNITY_IOS
using Steamworks;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    public static ActorData PlayerActor
    {
        get{return SocketHandler.Instance.CurrentUser.actor;}
    }

    public CanvasGroup GameUICG;

    public CGDatabase Data;

    public SceneInfo ActiveSceneInfo
    {
        get
        {
            return CORE.Instance.Data.content.Scenes.Find(X => SceneManager.GetActiveScene().name == X.sceneName);
        }
    }

    public RoomData Room;

    public PartyData CurrentParty;

    public PartyData CurrentGuild;

    public string TimePhase = "Day";
    
    public bool DEBUG = false;

    public bool DEBUG_SPAMMY_EVENTS = false;

    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

    public bool NEVER_BITCH = false;

    public bool CAN_MOVE_IN_SPELLS = false;

    public bool IsBitch;
    public bool InGame = false;
    public bool IsLoading = false;

    public bool IsPickingUpItem = false;
    public static bool IsMachinemaMode;
    public bool PhaseInitialized;
    public string CurrentTimePhase;

    public bool IsUsingJoystick;
    

    public bool IsTyping
    {
        get
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            if(KeyBindingWindowUI.Instance != null && KeyBindingWindowUI.Instance.IsWaitingForKeyActive)
            {
                return true;
            }

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

    public bool IsInputEnabled
    {
        get //All must be lightweight conditions(!)
        {
            return !CORE.Instance.IsLoading
                && !CORE.Instance.IsTyping
                && !(IsUsingJoystick && VirtualKeyboard.VirtualKeyboard.Instance.IsTyping)
                && !CORE.Instance.HasWindowOpen
                && !CameraChaseEntity.Instance.IsFocusing
                && !DecisionContainerUI.Instance.IsActive
                && (DialogEntity.CurrentInstance == null || !DialogEntity.CurrentInstance.isActiveDialog);
        }
    }

    public string NextScenePrediction;

    public bool IsAppInBackground = false;

    public string CurrentLanguage = "";

    float CursorTimer = 0f;
    const float CURSOR_MAX_TIMER = 3f;


    const float MAX_TIME_AFK = 7200;

    float TimeAFK = 0f;

    public WindowInterface CurrentWindow;
    public Dictionary<WindowInterface, KeyCode> WindowToKeyMap = new Dictionary<WindowInterface, KeyCode>();

    private void Awake()
    {
        Instance = this;
        #if !UNITY_ANDROID && !UNITY_IOS
#if DEVELOPMENT_BUILD || UNITY_EDITOR 
        if (!GetComponent<SocketHandler>().RandomUser)
        {
#endif
            if (SteamAPI.RestartAppIfNecessary(new AppId_t(1780330)))
            {
                Application.Quit();
                return;
            }

            SteamAPI.Init();
            
            ConditionalInvokation((x) => { return SteamAPI.Init() && WarningWindowUI.Instance != null; }, () => 
            {
                LogMessage("Initializing Connection");
                //TODO - This is an old method, might aswell remove this section entirely
                //string connectLobbyUniqueKey = "";
                //int cmnd = SteamApps.GetLaunchCommandLine(out connectLobbyUniqueKey, 260);
                //if (!string.IsNullOrEmpty(connectLobbyUniqueKey))
                //{
                //    TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Handling your specific request!"+ connectLobbyUniqueKey+" | " + cmnd, Color.yellow, 1, false));
                //    Debug.LogError("1 SHOULD JOIN LOBBY " + connectLobbyUniqueKey);

                //    pendingJoinParty = connectLobbyUniqueKey;
                //}
            
                string[] args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Contains("connect_lobby"))
                    {
                        pendingJoinParty = args[i + 1];
                        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Will auto-join as soon as your enter the matching realm!", Color.green, 4, false));
                    }
                }


                CurrentLanguage = PlayerPrefs.GetString("language", "");

                if (string.IsNullOrEmpty(CurrentLanguage))
                {
                    Debug.Log("Using steam's UI language");
                    CurrentLanguage = SteamApps.GetCurrentGameLanguage();

                    if (CurrentLanguage == "Simplified Chinese")
                    {
                        CurrentLanguage = "Chinese (Simplified)";
                    }

                    if (CurrentLanguage == "Spanish - Spain")
                    {
                        CurrentLanguage = "Spanish";
                    }

                }

                PlayerPrefs.SetString("language", CurrentLanguage);
                PlayerPrefs.Save();

                LocalizationManager.CurrentLanguage = CurrentLanguage;
                CORE.Instance.InvokeEvent("LanguageChanged");


                AchievementLogic.Instance.StartAchievementLogic();
            });

            if(GetJoinRequestResponse == null)
                GetJoinRequestResponse = Callback<GameRichPresenceJoinRequested_t>.Create(OnGetJoinRequestResponse);

            if (GetJoinLobbyRequestResponse == null)
                GetJoinLobbyRequestResponse = Callback<GameLobbyJoinRequested_t>.Create(OnGetJoinLobbyRequestResponse);

            if (GetLobbyCreatedRespose == null)
                GetLobbyCreatedRespose = Callback<LobbyCreated_t>.Create(OnGetLobbyCreatedResponse);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        }
#endif
#endif



        Application.targetFrameRate = 60;
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        Time.fixedDeltaTime = 0.01666667f;
        Application.runInBackground = true;

        CurrentParty = null;
        CurrentGuild = null;

        DontDestroyOnLoad(this.gameObject);
    }


#if !UNITY_ANDROID && !UNITY_IOS
    protected Callback<GameLobbyJoinRequested_t> GetJoinLobbyRequestResponse;
    void OnGetJoinLobbyRequestResponse(GameLobbyJoinRequested_t pCallBack)
    {
        LogMessage("STEAM - JOIN LOBBY RESPONSE | key: " + pCallBack.m_steamIDLobby);
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Joining a friend's lobby!", Color.green, 3, true));

        pendingJoinParty = pCallBack.m_steamIDLobby.ToString();

        if (SocketHandler.Instance.SocketManager.State == BestHTTP.SocketIO.SocketManager.States.Open)
            CheckOOGInvitations();
    }

    protected Callback<GameRichPresenceJoinRequested_t> GetJoinRequestResponse;
    public string pendingJoinParty = null;
    void OnGetJoinRequestResponse(GameRichPresenceJoinRequested_t pCallBack)
    {
        LogMessage("STEAM - JOIN GAME RESPONSE | key: " + pCallBack.m_rgchConnect);
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Joining a friend's game!", Color.green, 3, true));

        pendingJoinParty = pCallBack.m_rgchConnect;

        if(SocketHandler.Instance.SocketManager.State == BestHTTP.SocketIO.SocketManager.States.Open)
            CheckOOGInvitations();
    }

    protected Callback<LobbyCreated_t> GetLobbyCreatedRespose;
    void OnGetLobbyCreatedResponse(LobbyCreated_t pCallBack)
    {
        if (pCallBack.m_ulSteamIDLobby == 0)
        {
            LogMessage("STEAM - LOBBY CREATION FAILED | " + pCallBack.m_eResult.ToString());
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Created Steam Lobby Failed!", Color.red, 3, true));
            return;
        }

        LogMessage("STEAM - LOBBY CREATED RESPONSE | key: " + pCallBack.m_ulSteamIDLobby);
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Lobby Created!", Color.green, 3, true));
       

        JSONNode node = new JSONClass();

        node["steamLobbyId"] = pCallBack.m_ulSteamIDLobby.ToString();
        SocketHandler.Instance.SendEvent("party_steam_lobby_id", node);
    }
#endif

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            AudioControl.Instance.SetNoInBackground();
            IsAppInBackground = false;
        }
        else
        {
            AudioControl.Instance.SetInBackground();
            IsAppInBackground = true;
        }
    }

    private void Start()
    {        
        SubscribeToEvent("ActorDied", () => { Room.RefreshThreat(); });
        SubscribeToEvent("ActorResurrected", () => { Room.RefreshThreat(); });
        SubscribeToEvent("ActorChangedStates", () => { Room.RefreshThreat(); });
        SubscribeToEvent("PhaseChanged", () => { PhaseChange(); });

        WindowToKeyMap.Add(AbilitiesUI.Instance, InputMap.Map["Abilities Window"]);
        WindowToKeyMap.Add(InventoryUI.Instance, InputMap.Map["Character Window"]);
        WindowToKeyMap.Add(FriendsWindowUI.Instance, InputMap.Map["Friends Window"]);
        WindowToKeyMap.Add(GuildWindowUI.Instance, InputMap.Map["Guild Window"]);
        WindowToKeyMap.Add(PartyWindowUI.Instance, InputMap.Map["Party Window"]);
        WindowToKeyMap.Add(QuestWindowUI.Instance, InputMap.Map["Quests Window"]);
        WindowToKeyMap.Add(MapWindowUI.Instance, InputMap.Map["Map Window"]);
        // WindowToKeyMap.Add(AlignmentWindowUI.Instance, InputMap.Map["Alignment Window"]);
        WindowToKeyMap.Add(SettingsMenuUI.Instance, InputMap.Map["Settings Window"]);
        WindowToKeyMap.Add(CashShopWindowUI.Instance,InputMap.Map["InApp Shop"]);
        WindowToKeyMap.Add(SideButtonUI.Instance, InputMap.Map["Exit"]);

        
        ReturnToMainMenu();


        DelayedInvokation(1f, () => { ValidateScreenRatio(); });
    }

    void ValidateScreenRatio()
    {
        
            #if !UNITY_ANDROID && !UNITY_IOS
        bool isScreenValid = (((float)Screen.width / (float)Screen.height) == 16f / 9f) || Mathf.Approximately(((float)Screen.width / (float)Screen.height), 16f / 9f);

        if(!isScreenValid)
        {
            LogMessage("Validating Screen Ratio");
            TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Readjusting Screen Ratio", Color.yellow, 3f, false));

            CORE.Instance.DelayedInvokation(1f, () => 
            {
                try
                {
                    GraphicSettingsHandler.Instance.fullScreenMode.value = (int)FullScreenMode.FullScreenWindow;
                    GraphicSettingsHandler.Instance.OnFullScreenModeChanged();
                    GraphicSettingsHandler.Instance.ApplySelectedResolution();
                    
                catch { }
            });
            //for(int i=0;i<Screen.resolutions.Length;i++)
            //{
            //    float currentRatio = ((float)Screen.resolutions[i].width / (float)Screen.resolutions[i].height);
            //    if (currentRatio == 16f / 9f || Mathf.Approximately(currentRatio, 16f / 9f))
            //    {
            //        CORE.Instance.DelayedInvokation(1f, () => { GraphicSettingsHandler.Instance.SetResolution(Screen.resolutions[i].width, Screen.resolutions[i].height); });
            //        return;
            //    }
            //}

            //CORE.Instance.DelayedInvokation(1f, () => { GraphicSettingsHandler.Instance.SetResolution(1280, 720); });
        }
        #endif
                
    }
    private void PhaseChange()
    {
        if (!PhaseInitialized)
        {
            PhaseInitialized = true;
            return;
        }


        if (TimePhase != CurrentTimePhase)
        {
            // if (!this.Room.HasEnemies && InGame)
            // {
            //     if (TimePhase == "Day")
            //     {
            //         ShowScreenEffect("ScreenEffectChamberToDay", null, false);
            //     }
            //     else if (TimePhase == "Night")
            //     {
            //         ShowScreenEffect("ScreenEffectChamberToNight", null, false);
            //     }
            // }


            CORE.Instance.ShowScreenEffect("ScreenEffectLocation", ActiveSceneInfo.displyName);

            CurrentTimePhase = TimePhase;
            RefreshSceneInfo();



        }
    }

    private void Update()
    {
        SetJoystickMode(Input.GetJoystickNames().Length > 0);
        
        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Cursor.visible = true;
            CursorTimer = CURSOR_MAX_TIMER;
        }

        if(CursorTimer > 0f)
        {
            CursorTimer -= 1f * Time.deltaTime;
        }
        else
        {
            Cursor.visible = false;
        }

        //TODO ADD LATER
        // if(SocketHandler.Instance != null && SocketHandler.Instance.SocketManager != null && SocketHandler.Instance.SocketManager.State == BestHTTP.SocketIO.SocketManager.States.Open) //AFK HANDLING
        // {
        //     if(Input.anyKey)
        //     {
        //         TimeAFK = 0f;
        //     }

            // if(TimeAFK < MAX_TIME_AFK)
            // {
            //     TimeAFK += Time.deltaTime;
            // }
            // else
            // {
            //     TimeAFK = 0f;
            //     ReturnToMainMenu();
            //     WarningWindowUI.Instance.Show("Disconnected due to inactivity...",()=>{});
            // }
        // }


        if (InGame && !IsLoading && !IsTyping)
        {
            foreach (var windowToKeyCode in WindowToKeyMap)
            {
                if (Input.GetKeyDown(windowToKeyCode.Value))
                {
                    ShowWindow(windowToKeyCode.Key, windowToKeyCode.Value, null, null);
                }
            }

            if (IsUsingJoystick && (Input.GetButtonDown("Joystick 8") ||  Input.GetButtonDown("Joystick 11")))
            {
                ShowSideButtonUiWindow();
            }
        }

        if (IsMachinemaMode && (Input.GetKeyDown(InputMap.Map["Exit"]) ||( Input.GetButtonDown("Joystick 8"))||  Input.GetButtonDown("Joystick 11")) && !CashShopWindowUI.Instance.IsOpen)
        {
            IsMachinemaMode = false;
            InvokeEvent("MachinemaModeRefresh");
        }

        if(Input.GetKeyDown(InputMap.Map["Hide GUI"]))
        {
            IsMachinemaMode = !IsMachinemaMode;
            InvokeEvent("MachinemaModeRefresh");
        }
    }

    public void ShowWindow(WindowInterface WindowToShow, KeyCode? keyPressed = null, ActorData ofActor = null, object data = null)
    {
        if(WindowToShow.GetType() == typeof(FriendsWindowUI))
        {
            return;
        }

        if(WindowToShow.GetType() == typeof(GuildWindowUI) &&  CurrentGuild == null)
        {
            return;
        }

        bool isTargetWindowClosed = CurrentWindow != WindowToShow;
        bool closedAWindow = CurrentWindow != null;
        CloseCurrentWindow();
        bool isClosingAWindowWithExit = closedAWindow && keyPressed == InputMap.Map["Exit"];
        if (isTargetWindowClosed && !isClosingAWindowWithExit)
        {
            CurrentWindow = WindowToShow;
            CurrentWindow.Show(ofActor == null ? SocketHandler.Instance.CurrentUser.actor : ofActor, data);
        }
    }


    public void ShowSettingsWindow()
    {
        ShowWindow(SettingsMenuUI.Instance);
    }
    public void ShowAbilitiesUiWindow()
    {
        ShowWindow(AbilitiesUI.Instance);
    }

    public void ShowInventoryUiWindow()
    {
        ShowWindow(InventoryUI.Instance);
    }

    public void ShowInventoryUiWindow(ActorData ofActor)
    {
        ShowWindow(InventoryUI.Instance, null, ofActor);
    }

    public void ShowPartyUiWindow()
    {
        ShowWindow(PartyWindowUI.Instance);
    }

    public void ShowQuestsUIWindow()
    {
        ShowWindow(QuestWindowUI.Instance);
    }

    public void ShowFriendsWindow()
    {
        ShowWindow(FriendsWindowUI.Instance);
    }

    public void ShowGuildWindow()
    {
        if(CurrentGuild == null)
        {
            WarningWindowUI.Instance.Show(CORE.QuickTranslate("You are not in a guild")+"!",null);
        }
        ShowWindow(GuildWindowUI.Instance);
    }

    public void ShowMapWindow()
    {
        ShowWindow(MapWindowUI.Instance);
    }

    public void ShowInAppShopWindow()
    {
        ShowWindow(CashShopWindowUI.Instance);
    }

    public void ShowAlignmentUiWindow()
    {
        ShowWindow(AlignmentWindowUI.Instance);
    }

    public void ShowSideButtonUiWindow()
    {
        ShowWindow(SideButtonUI.Instance);
    }

    public void ShowVendorSelectionWindow(ItemData currentItem)
    {
        ShowWindow(VendorSelectionUI.Instance, null, null, currentItem);
    }

    public void CloseCurrentWindow()
    {
        if (CurrentWindow != null)
        {
            CurrentWindow.Hide();
            CurrentWindow = null;
        }
    }

    public void ReportBug(Action onComplete = null)
    {
        InputLabelWindow.Instance.Show("Report a Bug", "What went wrong?", (string msg) => 
        {
            JSONNode node = new JSONClass();
            node["message"] = msg;

            SocketHandler.Instance.SendEvent("report_bug", node);

            onComplete?.Invoke();
        });
    }

    public static void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
            container.GetChild(0).SetParent(Instance.transform);
        }
    }

    public static void DestroyContainer(Transform container)
    {
        for(int i=0;i<container.childCount;i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public static string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
    }

    public static string StripHTML(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", String.Empty);
    }

    public static string QuickTranslate(string text)
    {
        if(LocalizationManager.CurrentLanguage.ToLower().Contains("english"))
        {
            return text;
        }

        return Instance.Data.Localizator.mSource.GetTranslationCodwise(text);
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
            CORE.Instance.LogMessageError("EVENT " + eventKey + " does not exist!");
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

    public Coroutine DelayedInvokation(float time, Action action)
    {
        Coroutine routine = null;
        routine = StartCoroutine(DelayedInvokationRoutine(time, action,routine));
        return routine;
    }

    IEnumerator DelayedInvokationRoutine(float time, Action action,Coroutine routine = null)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
        routine = null;
    }

    public void ConditionalInvokation(Predicate<object> condition, Action action, float interval = 1f, bool repeat = false)
    {
        StartCoroutine(ConditionalInvokationRoutine(condition, action, interval));
    }

    IEnumerator ConditionalInvokationRoutine(Predicate<object> condition, Action action, float interval = 1f, bool repeat = false)
    {

        while (!condition(null))
        {
            yield return new WaitForSeconds(interval);
        }

        action.Invoke();

        if (repeat)
        {
            StartCoroutine(ConditionalInvokationRoutine(condition, action, interval, repeat));
        }
    }

    public void LogMessage(string message)
    {
        if (!DEBUG)
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
        if (LoadSceneRoutineInstance != null)
        {
            StopCoroutine(LoadSceneRoutineInstance);
        }
        Room = new RoomData();

        LoadSceneRoutineInstance = StartCoroutine(LoadSceneRoutine(sceneKey, onComplete));
    }

    public void SpawnActor(ActorData actorData)
    {
        ActorData existingActor = Room.Actors.Find(X => X.actorId == actorData.actorId);

        if (existingActor != null && existingActor.ActorEntity != null && existingActor.ActorEntity.IsDead)
        {
            existingActor.ActorEntity.Resurrect();
            return;
        }

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


        if (dataRef == null)
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

        if (interactable == null)
        {
            LogMessageError("No interactable with the id " + interactableId);
            return;
        }

        interactable.Entity.Interacted(byActorID);
    }

    public void SpawnItem(Item item)
    {
        ItemEntity itemEntity;
        if(string.IsNullOrEmpty(item.Data.UniquePrefab))
        {
            itemEntity = ResourcesLoader.Instance.GetRecycledObject("WorldItem").GetComponent<ItemEntity>();
        }
        else
        {
            itemEntity = ResourcesLoader.Instance.GetRecycledObject(item.Data.UniquePrefab).GetComponent<ItemEntity>();
        }
        itemEntity.transform.position = new Vector2(item.x, item.y);
        item.Entity = itemEntity;
        itemEntity.SetInfo(item);

        AudioControl.Instance.PlayInPosition("itemDrop",item.Entity.transform.position);

        Room.ItemJoined(item);
    }

    public void DespawnItem(string itemId)
    {
        Room.ItemLeft(itemId);
    }

    public void DisposeSession()
    {
        CORE.Instance.CurrentParty = null;
        CORE.Instance.CurrentGuild = null;
        DefaultChatLogUI.Instance.ClearLog();
        ConsoleInputUI.Instance.ClearLog();
        LootRollPanelUI.Instance.ClearContainer();
        ExpeditionQueTimerUI.Instance.Hide();
        FriendsDataHandler.Instance.ClearFriends();
        QuestsPanelUI.Instance.Wipe();
        CORE.Instance.InvokeEvent("PartyUpdated");
    }

    public void DisposeChamberCache()
    {
        screenEffectQue.Clear();
        // Room = null;
    }

    Coroutine LoadSceneRoutineInstance;
    IEnumerator LoadSceneRoutine(string sceneKey, Action onComplete = null)
    {
        DisposeChamberCache();

        string currentSceneKey = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneKey);

        if (RoomUpdateRoutineInstance != null)
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


        InvokeEvent("NewSceneLoaded");

        PlayerPrefs.SetString(SceneManager.GetActiveScene().name+"_vl","true");
        PlayerPrefs.Save();

        if (sceneKey == "MainMenu")
        {
            if (InGame)
            {
                SocketHandler.Instance.SendDisconnectSocket();
                LeaveGame();
            }
        }
        else
        {
            EnterGame();
        }

        LoadSceneRoutineInstance = null;

        //    ObjectiveUI.Instance.SetInfo(Data.content.Scenes.Find(X => X.sceneName == sceneKey).objectiveDescription);

    }

    Coroutine PickupBusyRoutineInstance;
    public void AttemptPickUpItem(Item item)
    {
        if (IsPickingUpItem)
        {
            return;
        }

        SocketHandler.Instance.SendPickedItem(item.itemId);
        IsPickingUpItem = true;

        if(PickupBusyRoutineInstance != null)
        {
            StopCoroutine(PickupBusyRoutineInstance);
        }

        PickupBusyRoutineInstance = DelayedInvokation(1f,()=>
        {
            IsPickingUpItem = false; 
            PickupBusyRoutineInstance= null;
        });
    }

    void EnterGame()
    {
        GameUICG.alpha = 1f;
        GameUICG.interactable = true;
        GameUICG.blocksRaycasts = true;
        InGame = true;


    }

    void LeaveGame()
    {
        GameUICG.alpha = 0f;
        GameUICG.interactable = false;
        GameUICG.blocksRaycasts = false;
        InGame = false;
    }


    bool autoExpededOnce = false;
    public void CheckOOGInvitations()
    {
        if(SocketHandler.Instance.SocketManager.State == BestHTTP.SocketIO.SocketManager.States.Open)
        {
            
#if !UNITY_ANDROID && !UNITY_IOS
            if(!string.IsNullOrEmpty(pendingJoinParty))
            {
                JSONNode node = new JSONClass();
                node["steamLobbyId"] = pendingJoinParty;
                SocketHandler.Instance.SendEvent("party_auto_join", node);
                TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Trying to join party...", Color.green, 3, false));

            }

#endif
            if(!ExpeditionQueTimerUI.Instance.IsSearching && !autoExpededOnce)
            {
                SocketHandler.Instance.SendStartExpeditionQueue("Forest");
                CORE.Instance.ConditionalInvokation(X=>ExpeditionQueTimerUI.Instance.IsSearching,()=>{
                    autoExpededOnce = true;
                    ExpeditionQueTimerUI.Instance.gameObject.SetActive(false);
                });
            }

        }
    }

    public void SetJoystickMode(bool isOn)
    {
        if(IsUsingJoystick && !isOn)
        {
            IsUsingJoystick = false;
            //CHANGE STATE
        }
        else if (!IsUsingJoystick && isOn)
        {
            IsUsingJoystick = true;
            //CHANGE STATE

        
        }
    }

    public void ReturnToMainMenu()
    {
        LoadScene("MainMenu",()=> 
        {
            ResourcesLoader.Instance.RunWhenResourcesLoaded(() =>
            {
                AudioControl.Instance.SetMusic(Data.content.titleScreenMusic);
                AudioControl.Instance.SetSoundscape(Data.content.titleScreenSoundscape);
            });
        });
    }

    Coroutine RoomUpdateRoutineInstance;
    IEnumerator RoomUpdateRoutine()
    {
        while (true)
        {
            Room.SendActorsPositions();
            yield return 0;
        }
    }

    public void UpdateSteamStatus()
    {
#if DEVELOPMENT_BUILD  || UNITY_EDITOR
        if (SocketHandler.Instance.RandomUser)
        {
            return;
        }
#endif

#if !UNITY_ANDROID && !UNITY_IOS
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            SteamFriends.SetRichPresence("steam_display", "#Status_AtMainMenu");
        }
        else
        {
            if (SocketHandler.Instance.SelectedRealmIndex != -1)
            {
                string realmSuffix = Data.content.Realms[SocketHandler.Instance.SelectedRealmIndex].Name;

                if (SceneManager.GetActiveScene().name == "Sunset Port")
                {
                    SteamFriends.SetRichPresence("steam_display", "#Status_SunsetPort_" + realmSuffix);
                }
                else if (SceneManager.GetActiveScene().name == "SunsetPortTavern")
                {
                    SteamFriends.SetRichPresence("steam_display", "#Status_SunsetPortTavern_" + realmSuffix);
                }
                else if (Data.content.Expeditions.Find(X => X.Floors.Find(f => f.PossibleChambers.Find(c => c == SceneManager.GetActiveScene().name) != null) != null) != null) // Is there an expedition with that scene?
                {
                    SteamFriends.SetRichPresence("steam_display", "#Status_Expedition_" + realmSuffix);
                }
            }
        }

#endif
    }

    #region Screen Effects

    public class ScreenEffectQueInstance
    {
        public string Key;
        public object Data;
    }

    GameObject LastScreenEffect;
    List<ScreenEffectQueInstance> screenEffectQue = new List<ScreenEffectQueInstance>();

    public GameObject ShowScreenEffect(string screenEffectObject, object data = null, bool skipQue = false, float animSpeed = 1f)
    {
        if (!skipQue && LastScreenEffect != null)
        {
            ScreenEffectQueInstance queInst = new ScreenEffectQueInstance();
            queInst.Key = screenEffectObject;
            queInst.Data = data;
            screenEffectQue.Add(queInst);
            return null;//TODO Make sure it doesnt break ActorControlClient.cs
        }
        GameObject refObj = ResourcesLoader.Instance.GetObject(screenEffectObject);
        GameObject obj = null;

        if(refObj != null)
        {
            obj = Instantiate(refObj);
            obj.transform.SetParent(GameUICG.transform, true);
            obj.transform.position = GameUICG.transform.position;
            obj.transform.localScale = Vector3.one;
            ScreenEffectUI screenEffect = obj.GetComponent<ScreenEffectUI>();

            if(screenEffect != null)
            {
                screenEffect.Show(data);
            }

            Animator animer = obj.GetComponent<Animator>();
            if (animer != null)
            {
                animer.speed = animSpeed;
            }

            RectTransform rt = obj.GetComponent<RectTransform>();

            if(rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
            }
        }

        if (!skipQue)
        {
            LastScreenEffect = obj;

            DelayedInvokation(1f, () =>
            {
                StartCoroutine(NextScreenEffect());
            });
        }

        return obj;

    }

    public IEnumerator NextScreenEffect()
    {
        if(LastScreenEffect != null)
        {
            while (LastScreenEffect != null && LastScreenEffect.gameObject.activeInHierarchy)
            {
                yield return 0;
            }

            LastScreenEffect = null;
        }

        if (screenEffectQue.Count == 0)
        {
            yield break;
        }

        ScreenEffectQueInstance inst = screenEffectQue[0];
        screenEffectQue.RemoveAt(0);
        ShowScreenEffect(inst.Key, inst.Data);

    }

    #endregion

    public void AddChatMessage(string chatlogMessage, string channel = "all")
    {
        DefaultChatLogUI.Instance.AddLogMessage(chatlogMessage,channel);
        ConsoleInputUI.Instance.AddLogMessage(chatlogMessage,channel);
    }

    public void ActivateParams(List<AbilityParam> onExecuteParams, Actor casterActor = null, Actor originCaster = null)
    {
        foreach (AbilityParam param in onExecuteParams)
        {
            if (param.Condition != null && !param.Condition.IsValid(originCaster))
            {
                continue;
            }

            foreach (GameCondition GameCondition in param.GameConditions)
            {
                if (!GameCondition.IsValid(originCaster))
                {
                    continue;
                }
            }

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
            else if (param.Type.name == "Change Looks")
            {
                switch(param.Value)
                {
                    case "Ears":
                    {
                        casterActor.State.Data.looks.Ears = param.Value2;
                        break;
                    }
                    case "Eyes":
                    {
                        casterActor.State.Data.looks.Eyes = param.Value2;
                        break;
                    }
                    case "Nose":
                    {
                        casterActor.State.Data.looks.Nose = param.Value2;
                        break;
                    }
                    case "Eyebrows":
                    {
                        casterActor.State.Data.looks.Eyebrows = param.Value2;
                        break;
                    }
                    case "Mouth":
                    {
                        casterActor.State.Data.looks.Mouth = param.Value2;
                        break;
                    }
                    case "Hair":
                    {
                        casterActor.State.Data.looks.Hair = param.Value2;
                        break;
                    }
                    case "Iris":
                    {
                        casterActor.State.Data.looks.Iris = param.Value2;
                        break;
                    }
                    case "HairColor":
                    {
                        casterActor.State.Data.looks.HairColor = param.Value2;
                        break;
                    }
                    case "SkinColor":
                    {
                        casterActor.State.Data.looks.SkinColor = param.Value2;
                        break;
                    }
                }

                                    
                casterActor.RefreshLooks();
            }
            else if(param.Type.name == "SetAchievement")
            {
                #if !UNITY_ANDROID && !UNITY_IOS
                AchievementLogic.Instance.SetAchievment(param.Value);
                #endif
            }
            else  if (param.Type.name == "Flip Screen")
            {
                CameraChaseEntity.Instance.transform.rotation = Quaternion.Euler(0f,0f,180f);
            }
            else  if (param.Type.name == "Unflip Screen")
            {
                CameraChaseEntity.Instance.transform.rotation = Quaternion.Euler(0f,0f,0f);
            }
        }
    }

    public void RefreshSceneInfo()
    {
        SceneInfo info = ActiveSceneInfo;

        if (info != null)
        {
            if (CORE.Instance.TimePhase == "Day")
            {
                if (!string.IsNullOrEmpty(info.MusicTrack))
                {
                    AudioControl.Instance.SetMusic(info.MusicTrack);
                }

                if (!string.IsNullOrEmpty(info.Soundscape))
                {
                    AudioControl.Instance.SetSoundscape(info.Soundscape);
                }
                else
                {
                    AudioControl.Instance.SetSoundscape(null);
                }
            }
            else if (CORE.Instance.TimePhase == "Night")
            {
                if (!string.IsNullOrEmpty(info.NightMusicTrack))
                {
                    AudioControl.Instance.SetMusic(info.NightMusicTrack);
                }
                else
                {
                    if (!string.IsNullOrEmpty(info.MusicTrack))
                    {
                        AudioControl.Instance.SetMusic(info.MusicTrack);
                    }
                }

                if (!string.IsNullOrEmpty(info.NightSoundscape))
                {
                    AudioControl.Instance.SetSoundscape(info.NightSoundscape);
                }
                else
                {
                    if (!string.IsNullOrEmpty(info.Soundscape))
                    {
                        AudioControl.Instance.SetSoundscape(info.Soundscape);
                    }
                    else
                    {
                        AudioControl.Instance.SetSoundscape(null);
                    }
                }
            }


        }
    }
}

[Serializable]
public class PartyData
{
    public string partyId;
    public ulong steamLobbyId;
    public string leaderName;
    public string[] members;
    public Dictionary<string, string[]> scenesToMembers;
    public Dictionary<string, bool> membersOffline;

    [JsonIgnore]
    public bool IsPlayerLeader
    {
        get
        {
            return CORE.Instance.Room.PlayerActor.name == leaderName;
        }
    }
}

[Serializable]
public class RoomData
{
    public List<ActorData> Actors = new List<ActorData>();
    public List<Interactable> Interactables = new List<Interactable>();
    public List<Item> Items = new List<Item>();
    public Dictionary<string, List<Item>> Vendors = new Dictionary<string, List<Item>>();

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
            return Actors.Find(x => x.isMob && x.ActorEntity != null && !x.ActorEntity.IsDead && !x.ActorEntity.IsHarmless) != null;
        }
    }

    public Actor GetMostThreateningActor()
    {
        Actor mostThreatAct = null;
        float mostThreat = Mathf.NegativeInfinity;
        for (int i = 0; i < Actors.Count; i++)
        {
            if(Actors[i] == null || Actors[i].ActorEntity == null)
            {
                continue;
            }

            if (Actors[i].ActorEntity.IsDead || Actors[i].isMob || Actors[i].states.ContainsKey("Untargetable"))
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
            if (Actors[i].ActorEntity.IsDead || Actors[i].isMob || Actors[i].states.ContainsKey("Untargetable"))
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

    public Actor GetFurthestActor(Actor from, bool LookForPlayer, float maxYDistance = 12, float maxXDistance = 50)
    {
        return GetDistancedActor(from, LookForPlayer, false, maxYDistance, maxXDistance);
    }

    public Actor GetNearestActor(Actor from, bool LookForPlayer, float maxYDistance = 12, float maxXDistance = 50)
    {
        return GetDistancedActor(from, LookForPlayer, true, maxYDistance, maxXDistance);
    }

    public Actor GetDistancedActor(Actor from, bool LookForPlayer, bool IsNearest, float maxYDistance = 12, float maxXDistance = 50)
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
            if (maxYDistance > 0 && yDistance > maxYDistance)
            {
                continue;
            }

            // The to actor is too far left/right
            float xDistance = Mathf.Abs(from.transform.position.x - to.transform.position.x);
            if (maxXDistance > 0 && xDistance > maxXDistance)
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

        if (actor.IsPlayer)
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

        if (actor == null)
        {
            CORE.Instance.LogMessageError("No actorId " + actorID + " in room.");
            return;
        }

        if(actor.ActorEntity == null)
        {
            CORE.Instance.LogMessageError("No actorId " + actorID + " ENTITY in room.");
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

    public void RefreshVendors(List<Vendor> vendors)
    {
        foreach (Vendor vendor in vendors)
        {
            if (!Vendors.ContainsKey(vendor.id))
            {
                Vendors.Add(vendor.id, vendor.itemsPool);
            }
            else
            {
                Vendors[vendor.id] = vendor.itemsPool;
            }

            CORE.Instance.InvokeEvent("VendorsUpdate" + vendor.id);
        }
    }


    public void RefreshThreat()
    {
        MostThreateningActor = GetMostThreateningActor();
        LeastThreatheningActor = GetLeastThreateningActor();
    }

    int lastSentMovementDirection;
    public void SendActorsPositions()
    {
        List<ActorData> actorsToUpdate = new List<ActorData>();
        JSONNode node = new JSONClass();
        for (int i = 0; i < Actors.Count; i++)
        {
            ActorData actor = Actors[i];
            if ((actor.IsPlayer || (!actor.isCharacter && CORE.Instance.IsBitch)) && actor.ActorEntity != null)
            {
                float lastX = actor.x;
                float lastY = actor.y;
                bool lastFaceRight = actor.faceRight;

                actor.x = actor.ActorEntity.transform.position.x;
                actor.y = actor.ActorEntity.transform.position.y;
                //actor.movementDirection = actor.ActorEntity.ClientMovingTowardsDir;

                actor.faceRight = actor.ActorEntity.Body.localScale.x < 0f;

                if (lastX != actor.x || lastY != actor.y || lastFaceRight != actor.faceRight || lastSentMovementDirection != actor.movementDirection)
                {
                    actorsToUpdate.Add(actor);
                }

                lastSentMovementDirection = actor.movementDirection;
            }
        }

        for (int i = 0; i < actorsToUpdate.Count; i++)
        {
            ActorData actor = actorsToUpdate[i];
            node["actorPositions"][i]["actorId"] = actor.actorId;
            node["actorPositions"][i]["x"].AsFloat = actor.x;
            node["actorPositions"][i]["y"].AsFloat = actor.y;
            node["actorPositions"][i]["faceRight"].AsBool = actor.faceRight;
            node["actorPositions"][i]["movementDirection"].AsInt = actor.movementDirection;
        }

        if (actorsToUpdate.Count > 0)
        {
            SocketHandler.Instance.SendEvent("actors_moved", node);
        }
        
    }

    public void ReceiveActorPositions(JSONNode data)
    {
        for (int i = 0; i < data["actorPositions"].Count; i++)
        {
            ActorData actor = Actors.Find(x => x.actorId == data["actorPositions"][i]["actorId"].Value);

            if (actor == null)
            {
                CORE.Instance.LogMessageError("No actor with id " + data["actorPositions"][i]["actorId"].Value);
                continue;
            }

            if(actor.IsPlayer)
            {
                continue;
            }

            actor.x = float.Parse(data["actorPositions"][i]["x"]);
            actor.y = float.Parse(data["actorPositions"][i]["y"]);
            actor.faceRight = bool.Parse(data["actorPositions"][i]["faceRight"]);
            actor.movementDirection = data["actorPositions"][i]["movementDirection"].AsInt;
        }
    }

   
}
