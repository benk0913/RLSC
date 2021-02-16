using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{

    [SerializeField]
    Actor CurrentActor;

    [SerializeField]
    Image ImageFill;

    [SerializeField]
    CanvasGroup CG;

    float LastHpPercent = 1f;
    Coroutine UpdateBarFillRoutineInstance;

    public void SetCurrentActor(Actor actor)
    {
        CurrentActor = actor;
    }

    void Update()
    {
        if(CurrentActor == null)
        {
            return;
        }

        if (CurrentActor.State.Data.hp >= CurrentActor.State.Data.MaxHP || CurrentActor.State.Data.hp <= 0)
        {
            CG.alpha = 0f;
        }
        else
        {
            
            CG.alpha = 1f;
        }

        float hpPercent = (float)CurrentActor.State.Data.hp / CurrentActor.State.Data.MaxHP;
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
    
    IEnumerator UpdateBarFillRoutine()
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
