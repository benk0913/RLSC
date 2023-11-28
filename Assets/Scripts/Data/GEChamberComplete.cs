using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEChamberComplete ", menuName = "Data/GEChamberComplete ", order = 2)]
public class GEChamberComplete : GameEvent
{
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        JSONNode node = new JSONClass();
        node["showEffect"].AsBool = true;
        SocketHandler.Instance.OnExpeditionFloorComplete("expedition_floor_complete",node);
    }
}
