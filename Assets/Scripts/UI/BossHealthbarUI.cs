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
        return CurrentActor == null || CurrentActor.State.Data.hp <= 0;
    }
}
