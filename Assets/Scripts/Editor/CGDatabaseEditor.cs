using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

[CustomEditor(typeof(CGDatabase))]
public class CGDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CGDatabase db = (CGDatabase)target;

        if (GUILayout.Button("Sync With Server"))
        {
            SendWebRequest(db.HostURL, JsonConvert.SerializeObject(db, Formatting.None));
        }
        
        DrawDefaultInspector();
    }

    public void SendWebRequest(string url, string sentJson = "")
    {
        UnityWebRequest request;
        
        Debug.Log("Request: " + url + " | " + sentJson);
        
        request = UnityWebRequest.Post(url, new WWWForm());


        if (!string.IsNullOrEmpty(sentJson))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(sentJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.uploadHandler.contentType = "application/json";
            request.SetRequestHeader("Content-Type", "application/json");
        }

        

        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        operation.completed += (AsyncOperation op) => 
        {
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
                return;
            }

            Debug.Log("Response: " + url + " | " + request.downloadHandler.text);
        };



        

    }

}