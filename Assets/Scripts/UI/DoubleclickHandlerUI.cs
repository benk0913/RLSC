using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DoubleclickHandlerUI : MonoBehaviour, IPointerClickHandler
{
    Coroutine DoubleClickRoutineInstance;

    [SerializeField]
    public UnityEvent OnDoubleClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClick();
    }

    public void OnPointerClick()
    {
        if(DoubleClickRoutineInstance != null)
        {
            OnDoubleClick?.Invoke();
            return;
        }

        DoubleClickRoutineInstance = StartCoroutine(DoubleClickRoutine());
    }

    IEnumerator DoubleClickRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        DoubleClickRoutineInstance = null;
    }
}
