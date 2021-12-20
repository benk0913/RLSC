using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEShowScrap ", menuName = "Data/GEShowScrap ", order = 2)]
public class GEShowScrap : GameEvent
{
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);
        ScrapWindowUI.Instance.Show();
    }
}
