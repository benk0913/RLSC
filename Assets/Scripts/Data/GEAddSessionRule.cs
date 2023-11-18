using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEAddSessionRule ", menuName = "Data/GEAddSessionRule ", order = 2)]
public class GEAddSessionRule : GameEvent
{
    public string RuleKey;
    public bool Remove;
    
    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        if (!Remove)
        {
            CORE.Instance.AddSessionRule(RuleKey);
        }
        else
        {
            CORE.Instance.RemoveSessionRule(RuleKey);
        }
    }
}
