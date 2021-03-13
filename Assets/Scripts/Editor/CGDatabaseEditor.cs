using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CGDatabase))]
public class CGDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CGDatabase db = (CGDatabase)target;

        if (GUILayout.Button("Autofill Current Scene Data"))
        {
            AutofillCurrentSceneInfo(db);
        }

        if (GUILayout.Button("Sync With Server"))
        {
            AutofillDatabase(db);
            SendWebRequest(db.HostURL, JsonConvert.SerializeObject(db, Formatting.None));
        }
        
        DrawDefaultInspector();
    }
    
    public void AutofillCurrentSceneInfo(CGDatabase db)
    {

        SceneInfo currentInfo = db.content.Scenes.Find(x => x.sceneName == EditorSceneManager.GetActiveScene().name);

        if (currentInfo == null)
        {
            currentInfo = new SceneInfo();
            currentInfo.sceneName = EditorSceneManager.GetActiveScene().name;

            db.content.Scenes.Add(currentInfo);
        }

        SpawnerEntity[] spawners = FindObjectsOfType<SpawnerEntity>();

        currentInfo.Mobs.Clear();

        foreach (SpawnerEntity spawner in spawners)
        {
            if(spawner.PlayersSpawner)
            {
                currentInfo.playerSpawnX = spawner.transform.position.x;
                currentInfo.playerSpawnY = spawner.transform.position.y;
                continue;
            }

            MobSpawn spawn = new MobSpawn();
            spawn.monsterName = spawner.MobKey;
            spawn.positionX = spawner.transform.position.x;
            spawn.positionY = spawner.transform.position.y;
            spawn.respawnSeconds = spawner.RespawnSeconds;

            currentInfo.Mobs.Add(spawn);
        }

        EditorUtility.SetDirty(db);
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

        guids = AssetDatabase.FindAssets("t:InteractableData", new[] { "Assets/" + db.DataPath });
        db.content.Interactables.Clear();
        foreach (string guid in guids)
        {
            db.content.Interactables.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(InteractableData)) as InteractableData);
        }

        guids = AssetDatabase.FindAssets("t:Expedition", new[] { "Assets/" + db.DataPath });
        db.content.Expeditions.Clear();
        foreach (string guid in guids)
        {
            db.content.Expeditions.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Expedition)) as Expedition);
        }

        guids = AssetDatabase.FindAssets("t:SkinSet", new[] { "Assets/" + db.DataPath });
        db.content.Visuals.SkinSets.Clear();
        foreach (string guid in guids)
        {
            db.content.Visuals.SkinSets.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(SkinSet)) as SkinSet);
        }

        db.content.ExpChart.Clear();
        for (int lvl = 1; lvl <= db.content.MaxLevel; lvl++) {
            db.content.ExpChart.Add(calculateExpToTargetLevel(lvl));
        }

        EditorUtility.SetDirty(db);
    }

    private int calculateExpToTargetLevel(int lvl)
    {
        return (int)(50 * (Mathf.Pow(lvl, 3) - 6 * Mathf.Pow(lvl, 2) + 17 * lvl - 12) / 3);
    }
}