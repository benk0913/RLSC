using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEEnterExpeiditionQue", menuName = "Data/GEEnterExpeiditionQue", order = 2)]
public class GEEnterExpeiditionQue : GameEvent
{
    public string ExpeditionName = "Forest";
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        SocketHandler.Instance.SendStartExpeditionQueue(ExpeditionName);
    }
}
