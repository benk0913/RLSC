using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;

class DeployServerWindowsCustom : EditorWindow
{
    ServerEnvironment ServerEnvironment = new ServerEnvironment();
    bool initialConfirmationGroupEnabled;
    bool secondConfirmationGroupEnabled;
    bool disconnectedAllGroupEnabled;
    bool testedProdGroupEnabled;
    
    string disconnectWarning = "The server will be going down for maintenance in a minute";

    [MenuItem ("Build/Deploy Servers")]
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(DeployServerWindowsCustom));
    }
    
    void OnGUI ()
    {
        ServerEnvironment.Environment = "Prod";

        GUILayout.Space(20);
        GUILayout.Label("~~~~~~Deploy Server~~~~~~", EditorStyles.largeLabel);
        GUILayout.Label("*LIVE SERVER, BE CAREFUL");
        GUILayout.Space(10);

        initialConfirmationGroupEnabled = EditorGUILayout.BeginToggleGroup("I understand, let me through", initialConfirmationGroupEnabled);
        
        disconnectWarning = GUILayout.TextField(disconnectWarning);
        if (GUILayout.Button("Warn about servers going down"))
        {
            UpdateServerStatus("warn", disconnectWarning);
        }

        GUILayout.Space(10);

        secondConfirmationGroupEnabled = EditorGUILayout.BeginToggleGroup("People have been warned enough time before", secondConfirmationGroupEnabled);

        if (GUILayout.Button("Disconnect ALL users & prevent login (~20 seconds)"))
        {
            UpdateServerStatus("off");
        }

        GUILayout.Space(10);
        
        disconnectedAllGroupEnabled = EditorGUILayout.BeginToggleGroup("I verified that you can't login on any user", disconnectedAllGroupEnabled);
        
        if (GUILayout.Button("Update PROD server, code + CG (~5 minutes)"))
        {
            UpdateServerCode();
            UpdateCG();
        }


        GUILayout.Space(10);

        testedProdGroupEnabled = EditorGUILayout.BeginToggleGroup("I verified that production works on VIP users with the new steam build", testedProdGroupEnabled);
        
        if (GUILayout.Button("Enable login for everyone (~20 seconds)"))
        {
            UpdateServerStatus("on");
        }


        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.EndToggleGroup();
    }

    void UpdateServerStatus(string status, string warning = "")
    {
        JSONNode node = new JSONClass();
        
        node["status"] = status;
        node["warning"] = warning;
        node["unic0rn"] = ServerEnvironment.unic0rn;

        WebRequest.SendWebRequest(ServerEnvironment.HostUrl + "/update-login-status", node.ToString());
    }

    void UpdateServerCode()
    {
        JSONNode node = new JSONClass();
        
        node["unic0rn"] = ServerEnvironment.unic0rn;

        WebRequest.SendWebRequest(ServerEnvironment.HostUrl + "/update-server", node.ToString());
    }

    void UpdateCG()
    {
        CGDatabaseEditor.ForceSyncCG(ServerEnvironment);
    }
    
}