using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEShowBank ", menuName = "Data/GEShowBank ", order = 2)]
public class GEShowBank : GameEvent
{
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        BankPanelUI.Instance.Show();
    }
}
