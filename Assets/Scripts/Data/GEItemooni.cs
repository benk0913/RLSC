using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEItemooni ", menuName = "Data/GEItemooni ", order = 2)]
public class GEItemooni : GameEvent
{
    public ItemData targetItem;
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        JSONNode node = new JSONClass();
        node["item_name"] = targetItem.name;
        SocketHandler.Instance.SendEvent("ge_get_item", node);
    }
}
