using Newtonsoft.Json;
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
    public static SocketHandler Instance;

    public string HostUrl = "www.lul2.herokuapp.com";

    public UserData User;

    private void Awake()
    {
        Instance = this;
    }

    #region TEST

    public bool Test;
    private void Update()
    {
        if(Test)
        {
            SendWebRequest(HostUrl+ "/login",(UnityWebRequest lreq) =>
            {
                OnLogin(lreq);

                Dictionary<string, string> urlParams = new Dictionary<string, string>();
                
                urlParams["unicorn"] = User.Unicorn;
                urlParams["element"] = "fire";

                SendWebRequest(HostUrl + "/create-char", (UnityWebRequest ccreq) =>
                {
                    OnCreateCharacter(ccreq);

                }, 
                "",
                urlParams);
            });

            Test = false;
        }
    }

    #endregion
    
    #region Response

    public void OnLogin(UnityWebRequest response)
    {
        JSONNode data = JSON.Parse(response.downloadHandler.text); //Using an irellevant datastructure for simplisity's sake

        User.Unicorn = data["unicorn"].Value;
    }

    public void OnCreateCharacter(UnityWebRequest response)
    {   
        CreateCharacterResponseData data = JsonConvert.DeserializeObject<CreateCharacterResponseData>(response.downloadHandler.text);

        User.Character = data.Character;

        Debug.LogError(User.Character.name);
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
                Debug.Log("Request: " + urlWithParams + " | " + sentJson);
            }

            request = UnityWebRequest.Get(urlWithParams);
        }
        else
        {
            if (CORE.Instance.DEBUG)
            {
                Debug.Log("Request: " + url + " | " + sentJson);
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
            Debug.LogError(request.error);
            yield break;
        }

        if (CORE.Instance.DEBUG)
        {
            Debug.Log("Response: " + url + " | " + request.downloadHandler.text);
        }


        OnResponse?.Invoke(request);

    }

    #endregion
}


[Serializable]
public class UserData
{
    public string Unicorn;
    public CharacterData Character;
}

[Serializable]
public class CreateCharacterResponseData
{
    public CharacterData Character;
}

[Serializable]
public class CharacterData
{
    public string scene;
    public string name;
    public string element;
}
