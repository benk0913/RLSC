using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
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

public class SocketHandler : MonoBehaviour
{
    #region Essentials

    public static SocketHandler Instance;

    public bool IsLocal = false;
    public string LocalHostUrl = "http://localhost:5000";
    public string LocalSocketUrl = "http://localhost:5000/socket.io/";
    public string ProdHostUrl = "https://lul2.herokuapp.com";
    public string ProdSocketUrl = "https://lul2.herokuapp.com/socket.io/";
    public string HostUrl { get { return IsLocal ? LocalHostUrl : ProdHostUrl; }}
    public string SocketUrl { get { return IsLocal ? LocalSocketUrl : ProdSocketUrl; }}

    public UserData CurrentUser;

    public SocketManager SocketManager;

    public List<SocketEventListener> SocketEventListeners = new List<SocketEventListener>();

    private void Awake()
    {
        Instance = this;

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
        SocketEventListeners.Add(new SocketEventListener("connect_error", OnError));
        SocketEventListeners.Add(new SocketEventListener("error", OnError));
        SocketEventListeners.Add(new SocketEventListener("event_error", OnError));
        SocketEventListeners.Add(new SocketEventListener("connect_timeout", OnError));
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
    }

    public void RemoveListeners()
    {
        foreach (SocketEventListener listener in SocketEventListeners)
        {
            SocketManager.Socket.Off(listener.EventKey, listener.Callback);
        }

        SocketManager.Socket.Off(SocketIOEventTypes.Error, OnErrorRawCallback);
    }

    #endregion



    #region Request

    public void SendLogin(Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Connecting", Color.green, 3f, true));
        SendWebRequest(HostUrl + "/login", (UnityWebRequest lreq) =>
        {
            OnLogin(lreq);

            OnComplete?.Invoke();
        },"",null, true);
    }

    public void SendCreateCharacter(string element = "fire", Action OnComplete = null)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Creating Character...", Color.green, 1f, true));

        //Dictionary<string, string> urlParams = new Dictionary<string, string>();

        //urlParams["unicorn"] = CurrentUser.Unicorn;
        //urlParams["classJob"] = element;

        JSONNode node = new JSONClass();
        node["unicorn"] = CurrentUser.Unicorn;
        node["classJob"] = element;

        SendWebRequest(HostUrl + "/create-char", (UnityWebRequest ccreq) =>
        {
            OnCreateCharacter(ccreq);


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
        options.AdditionalQueryParams.Add("unicorn", CurrentUser.Unicorn);
        options.AdditionalQueryParams.Add("charIndex", CurrentUser.SelectedCharacterIndex.ToString());

        #if UNITY_EDITOR
        options.AdditionalQueryParams.Add("isEditor", "1");
        #endif

        options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;

        SocketManager = new SocketManager(new Uri(SocketUrl),options);
        SocketManager.Encoder = new SimpleJsonEncoder();

        AddListeners();

        SocketManager.Open();
        
        while(SocketManager.State != SocketManager.States.Open)
        {
            yield return 0;
        }

        OnComplete?.Invoke();

        CORE.Instance.LogMessage("Connected To Socket.");

        ConnectSocketRoutineInstance = null;
    }

    public void SendDisconnectSocket()
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Rejecting Humanity... Returning to MONKE...", Color.green, 3f, true));

        if (SocketManager != null)
        {
            SocketManager.Socket.Off();
            //RemoveListeners();
            SocketManager.Socket.Disconnect();
        }
        //SocketManager.Close();

        

        CORE.Instance.LogMessage("Disconnected From Socket.");
    }


    #endregion

    #region Response

    public void OnLogin(UnityWebRequest response)
    {
        JSONNode data = JSON.Parse(response.downloadHandler.text); //Using an irellevant datastructure for simplisity's sake

        CurrentUser.Unicorn = data["unicorn"].Value;
    }

    public void OnCreateCharacter(UnityWebRequest response)
    {
        //CurrentUser.actor = JsonConvert.DeserializeObject<ActorData>(response.downloadHandler.text);
        JSONNode data = JSON.Parse(response.downloadHandler.text);

        CurrentUser.actor = JsonConvert.DeserializeObject<ActorData>(data["actor"].ToString());
    }
    
    
    #endregion

    #region HTTP Request Handling

    public void SendWebRequest(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string,string> urlParams = null, bool isPost = false)
    {
        StartCoroutine(SendHTTPRequestRoutine(url, OnResponse,sentJson,urlParams,isPost));
    }

    public IEnumerator SendHTTPRequestRoutine(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null, bool isPost = false)
    {

        UnityWebRequest request;

        if (urlParams != null)
        {
            string urlWithParams = url;

            urlWithParams += "?" + urlParams.Keys.ElementAt(0) + "=" + urlParams[urlParams.Keys.ElementAt(0)];

            for (int i=1;i<urlParams.Keys.Count;i++)
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

        if (request.isNetworkError || request.isHttpError)
        {
            CORE.Instance.LogMessageError(request.error);
            yield break;
        }

        if (CORE.Instance.DEBUG)
        {
            CORE.Instance.LogMessage("Response: " + url + " | " + request.downloadHandler.text);
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

        JSONNode data;

        try
        {
            data = (JSONNode)args[0];
            CORE.Instance.LogMessageError("Socket IO error - "+packet.EventName+" | " + data.ToString());
        }
        catch
        {
            CORE.Instance.LogMessageError(string.Format("Casting Data to JSON Error... {0}", args[0]));
        }
    }

    public void OnError(string eventName, JSONNode data)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("ERROR "+data.ToString(), Color.red, 1f, true));

        CORE.Instance.LogMessageError("server error - " + data.ToString());
    }

    public void OnLoadScene(string eventName, JSONNode data)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Entering "+data["scene"].Value, Color.green, 1f, false));

        CORE.Instance.LoadScene(data["scene"].Value, ()=>
        {
            SendEvent("scene_loaded");
        });

        SceneInfo info = CORE.Instance.Data.content.Scenes.Find(X => X.sceneName == data["scene"].Value);
        if(info != null)
        {
            if(!string.IsNullOrEmpty(info.MusicTrack))
            {
                AudioControl.Instance.SetMusic(info.MusicTrack);
            }
        }
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

    public void OnMoveActors(string eventName, JSONNode data)
    {
        CORE.Instance.Room.ReceiveActorPositions(data);
    }

    protected void OnBitchPlease(string eventName, JSONNode data)
    {
        SendEvent("bitch_please",data);
    }

    protected void OnActorBitch(string eventName, JSONNode data)
    {
        CORE.Instance.IsBitch = data["is_bitch"].AsBool;
        CORE.Instance.InvokeEvent("BitchChanged");
    }


    public void OnActorSpawn(string eventName, JSONNode data)
    {
        ActorData actor = JsonConvert.DeserializeObject<ActorData>(data["actor"].ToString());

        if(CurrentUser.actor.actorId == actor.actorId)
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

        if(actorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["actorId"].Value);
            return;
        }

        string abilityName = data["abilityName"];

        if (!actorDat.IsPlayer)
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

        actorDat.ActorEntity.ExecuteAbility(ability,position,faceRight);

        
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

        actorDat.Attributes = JsonConvert.DeserializeObject<AttributeData>(data["attributes"].ToString());
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

        int dmg = data["dmg"].AsInt;
        actorDat.ActorEntity.ShowHurtLabel(dmg);

        string casterActorId = data["casterActorId"].Value;
        ActorData casterActorDat = CORE.Instance.Room.Actors.Find(x => x.actorId == casterActorId);

        if (casterActorDat == null)
        {
            CORE.Instance.LogMessageError("No actor with ID " + data["casterActorId"].Value);
            return;
        }
        casterActorDat.ActorEntity.AddDps(dmg);
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

        actorDat.ActorEntity.State.Interrupt(data["putAbilityOnCd"].AsBool);
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

        actorDat.States = JsonConvert.DeserializeObject<Dictionary<string,StateData>>(data["states"].ToString());

        if(actorDat.ActorEntity == null)
        {
            CORE.Instance.LogMessageError("ACTOR DATA HAS NO ENTITY?");
            return;
        }

        actorDat.OnRefreshStates?.Invoke();
    }
    
    #endregion
}


[Serializable]
public class UserData
{
    public string Unicorn;

    public ActorData actor;

    public int SelectedCharacterIndex;
}

[Serializable]
public class ActorData
{
    public string actorId;
    public string scene;
    public float x;
    public float y;
    public bool faceRight;
    public string name;
    public string classJob;
    public string actorType;
    public string prefab;
    public int hp;
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

    public AttributeData Attributes;

    public Dictionary<string, StateData> States = new Dictionary<string, StateData>();

    public List<string> abilities;

    [JsonIgnore]
    public float MovementSpeed
    {
        get
        {
            return CORE.Instance.Data.content.BaseAttributes.MovementSpeed + CORE.Instance.Data.content.BaseAttributes.MovementSpeed * Attributes.MovementSpeed;
        }
    }

    [JsonIgnore]
    public Actor ActorEntity;

    [JsonIgnore]
    public ClassJob ClassJobReference
    {
        get
        {
            if(_classjobRef == null && !string.IsNullOrEmpty(classJob))
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
    public float MaxHP
    {
        get
        {
            return CORE.Instance.Data.content.BaseAttributes.HP + CORE.Instance.Data.content.BaseAttributes.HP * Attributes.HP;
        }
    }

    [JsonIgnore]
    public UnityEvent OnRefreshStates = new UnityEvent();

    [JsonIgnore]
    public UnityEvent OnRefreshAbilities = new UnityEvent();

    public bool IsPlayer
    {
        get
        {
            return actorId == SocketHandler.Instance.CurrentUser.actor.actorId;
        }
    }

    public ActorData(string gScene, string gName, string gClassJob, GameObject gActorObject = null)
    {
        this.scene = gScene;
        this.name = gName;
        this.classJob = gClassJob;

        if (gActorObject != null)
        {
            this.ActorEntity = gActorObject.GetComponent<Actor>();
        }
    }
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
            if(_data == null)
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

    public Action<string,JSONNode> InternalCallback;


   

    public SocketEventListener(string key, Action<string,JSONNode> internalCallback = null)
    {
        this.EventKey = key;

        this.InternalCallback = internalCallback;
    }

    public void Callback(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data;

        //try
        //{
            data = JSON.Parse(args[0].ToString());
            //data = (JSONNode)args[0];
            InternalCallback.Invoke(packet.EventName,data);
        //}
        //catch
        //{
        //    CORE.Instance.LogMessageError(string.Format("Casting Data to JSON Error... {0}", args[0]));
        //}
    }

}

