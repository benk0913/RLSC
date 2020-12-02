using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    SpriteRenderer[] Renderers;

    private void Awake()
    {
        Renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public void SetColor(Color clr)
    {
        foreach(SpriteRenderer renderer in Renderers)
        {
            renderer.color = clr;
        }
    }
    
}
