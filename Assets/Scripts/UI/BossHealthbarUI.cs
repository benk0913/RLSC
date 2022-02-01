using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbarUI : HealthbarUI
{
    public static BossHealthbarUI Instance;

    [SerializeField]
    Animator Animer;


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

    protected override IEnumerator UpdateBarFillRoutine()
    {
        Animer.SetTrigger("Hit");

        float initialHpPercent = ImageFill.fillAmount;
        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime * 0.5f;
            // Use sin to ease the animation.
            float easedOutTime = Mathf.Sin(t * Mathf.PI);
            ImageFill.fillAmount = Mathf.Lerp(initialHpPercent, LastHpPercent, easedOutTime);

            yield return new WaitForFixedUpdate();
        }

        UpdateBarFillRoutineInstance = null;
    }
}
