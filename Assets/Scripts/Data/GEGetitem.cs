using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEGetItem ", menuName = "Data/GEGetItem", order = 2)]
public class GEGetItem : GameEvent
{
    public string ItemName;
    

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        JSONNode node = new JSONClass();
        node["item_name"] = ItemName;
        SocketHandler.Instance.SendEvent("ge_get_item", node);

    }
}
