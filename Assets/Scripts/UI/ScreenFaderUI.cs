using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFaderUI : MonoBehaviour
{
    public static ScreenFaderUI Instance;

    [SerializeField]
    Image FadeIMG;

    [SerializeField]
    float FadeSpeed = 1f;

    bool isFaded= false;

    Action OnFade;
    Action OnClear;

    private void Awake()
    {
        Instance = this;
    }

    public void FadeToBlack(Action onFade = null)
    {
        OnFade = onFade;

        isFaded = true;

        if (InterpolateRoutineInstance != null)
        {
            StopCoroutine(InterpolateRoutineInstance);
        }

        InterpolateRoutineInstance = StartCoroutine(InterpolateRoutine());
    }

    public void FadeFromBlack(Action onClear = null)
    {
        OnClear = onClear;

        isFaded = false;


        if (InterpolateRoutineInstance != null)
        {
            return;
        }

        InterpolateRoutineInstance = StartCoroutine(InterpolateRoutine());
    }

    Coroutine InterpolateRoutineInstance;

    IEnumerator InterpolateRoutine()
    {
        if (isFaded)
        {
            while (FadeIMG.color.a < 1f)
            {
                FadeIMG.color = new Color(FadeIMG.color.r, FadeIMG.color.g, FadeIMG.color.b, FadeIMG.color.a + (FadeSpeed * Time.deltaTime));
                yield return 0;
            }

            OnFade?.Invoke();
        }
        
        if(!isFaded) //Without else.
        {
            while (FadeIMG.color.a > 0f)
            {
                FadeIMG.color = new Color(FadeIMG.color.r, FadeIMG.color.g, FadeIMG.color.b, FadeIMG.color.a - (FadeSpeed * Time.deltaTime));
                yield return 0;
            }

            OnClear?.Invoke();
        }

        InterpolateRoutineInstance = null;
    }
}
