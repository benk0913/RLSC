using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using TMPro;
using Newtonsoft.Json;

[CustomEditor(typeof(CGDatabase))]
public class CGDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CGDatabase db = (CGDatabase)target;

        if (GUILayout.Button("Sync With Server"))
        {
            //Debug.Log(JsonUtility.ToJson(db));
            Debug.Log(JsonConvert.SerializeObject(db, Formatting.Indented));
        }

      

        DrawDefaultInspector();
    }

   
}