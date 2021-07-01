using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEStopExpeiditionQue", menuName = "Data/GEEnterExpeiditionQue", order = 2)]
public class GEStopExpeiditionQue : GameEvent
{
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        SocketHandler.Instance.SendStopExpeditionQueue();
    }
}
