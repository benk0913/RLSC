using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    SpriteRenderer[] Renderers;


    private void Awake()
    {
        Renderers = GetComponentsInChildren<SpriteRenderer>(true);

        List<SpriteRenderer> newRenderers = new List<SpriteRenderer>();//TODO Optimize the part from this point onward?

        foreach(SpriteRenderer renderer in Renderers)
        {
            if(renderer.material == CORE.Instance.Data.DefaultSpriteMaterial)
            {
                newRenderers.Add(renderer);
            }
        }

        Renderers = newRenderers.ToArray();
    }

    public void SetColor(Color clr)
    {
        foreach(SpriteRenderer renderer in Renderers)
        {
            renderer.color = clr;
        }
    }

    public void SetMaterial(Material mat)
    {
        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.material = mat;
        }
    }

    public void ResetMaterial()
    {
        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.material = CORE.Instance.Data.DefaultSpriteMaterial;
        }
    }
    
}
