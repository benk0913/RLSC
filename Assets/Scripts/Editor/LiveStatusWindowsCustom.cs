using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

class LiveStatusWindowsCustom : EditorWindow
{
    ServerEnvironment ServerEnvironment = new ServerEnvironment();

    static CGDatabase db
    {
        get
        {
            if (db_ == null)
            {
                db_ = CGDatabaseEditor.GetCGInEditor();
            }
            return db_;
        }
    }
    static CGDatabase db_;

    bool loading = false;
    
    List<ServerDetails> ServerDetails = new List<ServerDetails>();
    
    List<DisplayServer> DisplayServers = new List<DisplayServer>()
    {
        new DisplayServer("Server", 70, Server => db.content.Realms[Server.realmIndex].Name),
        new DisplayServer("Region", 60, Server => Server.region),
        new DisplayServer("Realm", 40, Server => "" + Server.realmIndex),
        new DisplayServer("Online", 40, Server => "" + Server.usersOnline),
        new DisplayServer("Queue", 40, Server => "" + Server.usersInQueue),
        new DisplayServer("Status", 40, Server => Server.status),
        new DisplayServer("Time", 40, Server => Server.displayTime),
        new DisplayServer("Version", 50, Server => "" + Server.version),
    };
    

    [MenuItem ("Build/Live Game Status")]
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(LiveStatusWindowsCustom));
    }
    
    void OnGUI ()
    {
        ServerEnvironment.Environment = "Prod";

        GUILayout.Space(20);

        GUILayout.Label("~~~~~~Live Game Status~~~~~~", EditorStyles.largeLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Refresh"))
        {
            RefreshLiveStatus();
        }

        GUILayout.Space(10);

        if (loading)
        {
            GUILayout.Label("Loading...");
        }
            
        if (ServerDetails.Count > 0)
        {
            GUILayout.Label("=========================");
            GUILayout.Label("Servers");
            GUILayout.Label("=========================");
            GUILayout.Space(8);


            for (int i = 0; i < DisplayServers.Count; i++)
            {
                DisplayServers[i].BuildTitleLabel(i == 0, i == DisplayServers.Count - 1);
            } 

            GUILayout.Space(4);
            
            foreach (var Server in ServerDetails)
            {
                
                for (int i = 0; i < DisplayServers.Count; i++)
                {
                    DisplayServers[i].BuildValueLabel(i == 0, i == DisplayServers.Count - 1, Server);
                } 
                GUILayout.Space(4);
            }
        }

    }

    void RefreshLiveStatus()
    {
        loading = true;
        ServerDetails.Clear();

        JSONNode node = new JSONClass();
        
        node["unic0rn"] = ServerEnvironment.unic0rn;

        WebRequest.SendWebRequest(ServerEnvironment.HostUrl + "/live-status", node.ToString(), (UnityWebRequest response) => 
        {
            JSONNode data = JSON.Parse(response.downloadHandler.text);

            loading = false;
            ServerDetails = JsonConvert.DeserializeObject<List<ServerDetails>>(data["servers"].ToString());
        });
    }
    
}

class ServerDetails
{
    public string region;
    public int realmIndex;
    public float usersOnline;
    public int usersInQueue;
    public string status;
    public string displayTime;
    public float version;
}

class DisplayServer
{
    public string FieldName;
    public int MaxLength;
    public Func<ServerDetails, string> DataGetter;

    public DisplayServer(string fieldName, int maxLength, Func<ServerDetails, string> dataGetter)
    {
        this.FieldName = fieldName;
        this.MaxLength = maxLength;
        this.DataGetter = dataGetter;
    }

    public void BuildTitleLabel(bool isFirst, bool isLast)
    {
        ConcatLabel(isFirst, isLast, FieldName, EditorStyles.boldLabel);
    }

    public void BuildValueLabel(bool isFirst, bool isLast, ServerDetails Server)
    {
        ConcatLabel(isFirst, isLast, DataGetter(Server), EditorStyles.label);
    }

    void ConcatLabel(bool isFirst, bool isLast, string value, GUIStyle Style)
    {
        if (isFirst)
        {
            GUILayout.BeginHorizontal();
        }
        else
        {
            GUILayout.Label("  |  ");
        }
        GUILayout.Label(value, Style, GUILayout.Width(MaxLength));
        
        if (isLast)
        {
            // Ensure that columns don't expand more than they need.
            GUILayout.Label(" ", GUILayout.Width(1000));
            GUILayout.EndHorizontal();
        }
    }
}