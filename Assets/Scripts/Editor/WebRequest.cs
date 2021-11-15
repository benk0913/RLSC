using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

class WebRequest
{
    
    public static void SendWebRequest(string url, string sentJson = "", Action<UnityWebRequest> OnResponse = null)
    {
        UnityWebRequest request;
        
        Debug.Log("Request: " + url + " | " + SocketHandler.FormatJson(sentJson));
        
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

            Debug.Log("Response: " + url + " | " + SocketHandler.FormatJson(request.downloadHandler.text));

            OnResponse?.Invoke(request);
        };
    }
}