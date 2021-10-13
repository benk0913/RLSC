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
    bool disconnectedAllGroupEnabled;
    bool waitedForUpdateGroupEnabled;
    bool testedProdGroupEnabled;

    [MenuItem ("Build/Deploy Servers")]
    public static void  ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(DeployServerWindowsCustom));
    }
    
    void OnGUI ()
    {
        ServerEnvironment.Environment = "Prod";
        ServerEnvironment.Region = "eu"; // VIP users are in EU

        GUILayout.Space(20);
        GUILayout.Label("~~~~~~Deploy Server~~~~~~", EditorStyles.boldLabel);
        GUILayout.Label("*LIVE SERVER, BE CAREFUL");

        initialConfirmationGroupEnabled = EditorGUILayout.BeginToggleGroup("I understand, let me through", initialConfirmationGroupEnabled);
        
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

        waitedForUpdateGroupEnabled = EditorGUILayout.BeginToggleGroup("I've waited 5 minutes", waitedForUpdateGroupEnabled);

        if (GUILayout.Button("Enable login for VIP users only (~20 seconds)"))
        {
            UpdateServerStatus("vip");
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

    void UpdateServerStatus(string status)
    {
        JSONNode node = new JSONClass();
        
        node["status"] = status;
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
        CGDatabaseEditor.ForceSyncCG();
    }
    
}