using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEScreenEffect", menuName = "Data/GEScreenEffect", order = 2)]
public class GEScreenEffect : GameEvent
{
    public string ScreenEffectKey;

    public override void Execute(System.Object obj = null)
    {
        CORE.Instance.ShowScreenEffect(ScreenEffectKey);
    }
}
