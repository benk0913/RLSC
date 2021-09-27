using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAssist : MonoBehaviour
{
    [SerializeField]
    ScrollRect rect;

    void Start()
    {
        if(rect == null)
        {
            rect = GetComponent<ScrollRect>();
        }
    }

    public void ScrollOnRect(float amount = 1f)
    {
        rect.verticalNormalizedPosition += amount *3f* Time.deltaTime;
    }

}
