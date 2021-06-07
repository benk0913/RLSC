using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioEntityUIHandle : AudioEntity, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public string PointerDownSound;
    public string PointerUpSound;
    public string PointerHoverSound;
    public string PointerUnhoverSound;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(string.IsNullOrEmpty(PointerDownSound))
        {
            return;
        }

        PlaySound(PointerDownSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(PointerHoverSound))
        {
            return;
        }

        PlaySound(PointerHoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(PointerUnhoverSound))
        {
            return;
        }

        PlaySound(PointerUnhoverSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(PointerUpSound))
        {
            return;
        }

        PlaySound(PointerUpSound);
    }
}
