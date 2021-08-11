using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbarUI : HealthbarUI
{
    public static BossHealthbarUI Instance;

    private void Awake()
    {
        Instance = this;
        
    }
    
    protected override bool ShouldHideBar()
    {
        if(CORE.IsMachinemaMode)
        {
            return true;
        }
        
        return CurrentActor == null || CurrentActor.State.Data.hp <= 0;
    }
}
