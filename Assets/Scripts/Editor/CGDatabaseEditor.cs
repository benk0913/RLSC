﻿using UnityEngine;
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

        if (GUILayout.Button("Autofill Database"))
        {
            AutofillDatabase(db);
        }

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

    public void AutofillDatabase(CGDatabase db)
    {
        string[] guids;

        guids = AssetDatabase.FindAssets("t:ClassJob", new[] { "Assets/" + db.DataPath });
        db.content.Classes.Clear();
        foreach (string guid in guids)
        {
            db.content.Classes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ClassJob)) as ClassJob);
        }

        guids = AssetDatabase.FindAssets("t:Ability", new[] { "Assets/" + db.DataPath });
        db.content.Abilities.Clear();
        foreach (string guid in guids)
        {
            db.content.Abilities.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Ability)) as Ability);
        }

        guids = AssetDatabase.FindAssets("t:Buff", new[] { "Assets/" + db.DataPath });
        db.content.Buffs.Clear();
        foreach (string guid in guids)
        {
            db.content.Buffs.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Buff)) as Buff);
        }

        guids = AssetDatabase.FindAssets("t:Monster", new[] { "Assets/" + db.DataPath });
        db.content.Monsters.Clear();
        foreach (string guid in guids)
        {
            db.content.Monsters.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Monster)) as Monster);
        }

    }

}