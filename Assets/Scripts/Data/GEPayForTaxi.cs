using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEPayForTaxi ", menuName = "Data/GEPayForTaxi ", order = 2)]
public class GEPayForTaxi : GameEvent
{
    public string targetScene = "Sunset Port";
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        JSONNode node = new JSONClass();
        node["target_scene"] = targetScene;
        SocketHandler.Instance.SendEvent("use_taxi", node);
    }
}
