﻿using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
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
    }

    private void Start()
    {
        SetupSocketIO();
    }

    void SetupSocketIO()
    {
        SocketManager = new SocketManager(new Uri(HostUrl));
        
        SocketEventListeners.Clear();
        

        SocketEventListeners.Add(new SocketEventListener("load_scene", OnLoadScene));
        SocketEventListeners.Add(new SocketEventListener("connect_error", OnError));
        SocketEventListeners.Add(new SocketEventListener("error", OnError));
        SocketEventListeners.Add(new SocketEventListener("connect_timeout", OnError));

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
    }

    public void RemoveListeners()
    {
        foreach (SocketEventListener listener in SocketEventListeners)
        {
            SocketManager.Socket.Off(listener.EventKey, listener.Callback);
        }
    }

    #endregion

    #region TEST

    public bool Test;
    private void Update()
    {
        if(Test)
        {
            SendLogin(() =>
                SendCreateCharacter(()=>
                    SendConnectSocket()));

            Test = false;
        }
    }

    private void OnApplicationQuit()
    {
        SendDisconnectSocket();
    }

    #endregion


    #region Request

    public void SendLogin(Action OnComplete = null)
    {
        SendWebRequest(HostUrl + "/login", (UnityWebRequest lreq) =>
        {
            OnLogin(lreq);

            OnComplete?.Invoke();
        });
    }

    public void SendCreateCharacter(Action OnComplete = null)
    {

        Dictionary<string, string> urlParams = new Dictionary<string, string>();

        urlParams["unicorn"] = CurrentUser.Unicorn;
        urlParams["classJob"] = "fire";

        SendWebRequest(HostUrl + "/create-char", (UnityWebRequest ccreq) =>
        {
            OnCreateCharacter(ccreq);


            OnComplete?.Invoke();
        },
        "",
        urlParams);
    }

    public void SendConnectSocket(Action OnComplete = null)
    {
        if(ConnectSocketRoutineInstance != null)
        {
            StopCoroutine(ConnectSocketRoutineInstance);
        }
        
        ConnectSocketRoutineInstance = StartCoroutine(ConnectSocketRoutine(OnComplete));
    }

    Coroutine ConnectSocketRoutineInstance;
    IEnumerator ConnectSocketRoutine(Action OnComplete = null)
    {
        BestHTTP.HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.All; //Uncomment to log socket...

        SocketOptions options = new SocketOptions();
        options.AdditionalQueryParams = new ObservableDictionary<string, string>();
        options.AdditionalQueryParams.Add("unicorn", CurrentUser.Unicorn);
        options.AdditionalQueryParams.Add("charIndex", "0");
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

        CurrentUser.actor.scene   = data["actor"]["scene"].Value;
        CurrentUser.actor.name    = data["actor"]["name"].Value;
        CurrentUser.actor.classJob = data["actor"]["classJob"].Value;


    }
    
    
    #endregion

    #region HTTP Request Handling

    public void SendWebRequest(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string,string> urlParams = null)
    {
        StartCoroutine(SendHTTPRequestRoutine(url, OnResponse,sentJson,urlParams));
    }

    public IEnumerator SendHTTPRequestRoutine(string url, Action<UnityWebRequest> OnResponse = null, string sentJson = "", Dictionary<string, string> urlParams = null)
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

            request = UnityWebRequest.Get(urlWithParams);
        }
        else
        {
            if (CORE.Instance.DEBUG)
            {
                CORE.Instance.LogMessage("Request: " + url + " | " + sentJson);
            }

            request = UnityWebRequest.Get(url);
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

    public void SendSceneLoaded()
    {
        JSONNode node = new JSONClass();
        
        SocketManager.Socket.Emit("scene_loaded",node);
    }

    #endregion

    #region Socket Response

    public void OnError(JSONNode data)
    {
        CORE.Instance.LogMessageError("server error - " + data.ToString());
    }

    public void OnLoadScene(JSONNode data)
    {
        CORE.Instance.LoadScene(data["scene"].Value, SendSceneLoaded);
    }

    #endregion
}


[Serializable]
public class UserData
{
    public string Unicorn;

    public ActorData actor;
}

[Serializable]
public class ActorData
{
    public string scene;
    public string name;
    public string classJob;
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
        JSONNode data = (JSONNode)args[0];
        InternalCallback.Invoke(data);
    }
}
