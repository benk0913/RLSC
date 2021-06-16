using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{

    [SerializeField]
    protected Actor CurrentActor;

    [SerializeField]
    protected Image ImageFill;

    [SerializeField]
    protected CanvasGroup CG;

    protected float LastHpPercent = 1f;
    protected Coroutine UpdateBarFillRoutineInstance;

    public void SetCurrentActor(Actor actor)
    {
        CurrentActor = actor;
        LastHpPercent = 1f;
        ImageFill.fillAmount = 1f;
    }

    protected void Update()
    {
        if (CurrentActor == null || CurrentActor.State == null || CurrentActor.State.Data == null || CurrentActor.State.Data.attributes == null) {
            return;
        }

        float hpPercent = (float)CurrentActor.State.Data.hp / CurrentActor.State.Data.MaxHP;

        if (ShouldHideBar())
        {
            CG.alpha = 0f;
            LastHpPercent = hpPercent;
            ImageFill.fillAmount = LastHpPercent;

            return;
        }
        else
        {
            CG.alpha = 1f;
        }

        if (hpPercent != LastHpPercent)
        {
            LastHpPercent = hpPercent;
            if(UpdateBarFillRoutineInstance != null)
            {
                StopCoroutine(UpdateBarFillRoutineInstance);
            }
            UpdateBarFillRoutineInstance = StartCoroutine(UpdateBarFillRoutine());
        }
    }

    protected virtual bool ShouldHideBar()
    {
        return CurrentActor == null || (CurrentActor.State.Data.isCharacter && !CurrentActor.InParty) ||( CurrentActor.State.Data.isMob && CurrentActor.State.Data.hp >= CurrentActor.State.Data.MaxHP) ;
    }

    protected IEnumerator UpdateBarFillRoutine()
    {
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
