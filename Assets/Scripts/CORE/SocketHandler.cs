﻿using BestHTTP.SocketIO;
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
using UnityEngine.Networking;

public class SocketHandler : MonoBehaviour
{
    #region Essentials

    public static SocketHandler Instance;

    public string HostUrl = "www.lul2.herokuapp.com";
    public string SocketUrl = "www.lul2.herokuapp.com";

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

        foreach (SocketEventListener listener in SocketEventListeners)
        {
            listener.InternalCallback += AddEventListenerLogging;
        }
    }

    void AddEventListenerLogging(JSONNode data)
    {
        CORE.Instance.LogMessage("On Socket Event: " + data["name"].Value+ " | " + data.ToString());
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

        CORE.Instance.LogMessage("Sending Event: " + eventKey + " | " + node.ToString());

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
            CORE.Instance.LogMessageError("Socket IO error - " + data.ToString());
        }
        catch
        {
            CORE.Instance.LogMessageError(string.Format("Casting Data to JSON Error... {0}", args[0]));
        }
    }

    public void OnError(JSONNode data)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("ERROR "+data.ToString(), Color.red, 1f, true));

        CORE.Instance.LogMessageError("server error - " + data.ToString());
    }

    public void OnLoadScene(JSONNode data)
    {
        TopNotificationUI.Instance.Show(new TopNotificationUI.TopNotificationInstance("Entering "+data["scene"].Value, Color.green, 1f, false));

        CORE.Instance.LoadScene(data["scene"].Value, ()=> SendEvent("scene_loaded"));
    }

    public void OnMoveActors(JSONNode data)
    {
        CORE.Instance.Room.ReceiveActorPositions(data);
    }

    protected void OnBitchPlease(JSONNode data)
    {
        SendEvent("bitch_please",data);
    }

    protected void OnActorBitch(JSONNode data)
    {
        CORE.Instance.IsBitch = data["is_bitch"].AsBool;
    }


    public void OnActorSpawn(JSONNode data)
    {
        ActorData actor = JsonConvert.DeserializeObject<ActorData>(data["actor"].ToString());

        CORE.Instance.SpawnActor(actor);
    }

    public void OnActorDespawn(JSONNode data)
    {
        CORE.Instance.DespawnActor(data["actorId"].Value);
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
    public float positionX;
    public float positionY;
    public string name;
    public string classJob;
    public string actorType;

    [JsonIgnore]
    public GameObject ActorObject;

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
        this.ActorObject = gActorObject;
    }
}

public class SocketEventListener
{
    public string EventKey;

    public Action<JSONNode> InternalCallback;


   

    public SocketEventListener(string key, Action<JSONNode> internalCallback = null)
    {
        this.EventKey = key;

        this.InternalCallback = internalCallback;
    }

    public void Callback(Socket socket, Packet packet, params object[] args)
    {
        JSONNode data;

        try
        {
            data = (JSONNode)args[0];
            InternalCallback.Invoke(data);
        }
        catch
        {
            CORE.Instance.LogMessageError(string.Format("Casting Data to JSON Error... {0}", args[0]));
        }
    }
}
