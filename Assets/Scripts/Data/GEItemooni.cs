using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEItemooni ", menuName = "Data/GEItemooni ", order = 2)]
public class GEItemooni : GameEvent
{
    public OnDemandParams OnDemandParams;

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        JSONNode node = new JSONClass();
        node["onDemandParamsId"] = OnDemandParams.name;
        SocketHandler.Instance.SendEvent("scene_on_demand_params", node);
    }
}
