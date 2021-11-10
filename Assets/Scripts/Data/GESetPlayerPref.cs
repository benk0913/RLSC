using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GESetPlayerPref ", menuName = "Data/GESetPlayerPref ", order = 2)]
public class GESetPlayerPref : GameEvent
{
    public string PrefKey;
    public string PrefValue;

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        PlayerPrefs.SetString(string.IsNullOrEmpty(PrefKey)?this.name:PrefKey,string.IsNullOrEmpty(PrefValue)?"true":PrefValue);
        PlayerPrefs.Save();
    }
}
