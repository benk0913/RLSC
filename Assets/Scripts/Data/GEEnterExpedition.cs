using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEEnterExpedition", menuName = "Data/GEEnterExpedition", order = 2)]
public class GEEnterExpedition : GameEvent
{
    public string ExpeditionName = "Forest";
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        SocketHandler.Instance.SendEnterExpedition(ExpeditionName);
    }
}
