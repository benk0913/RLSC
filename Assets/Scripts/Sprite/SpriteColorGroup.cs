using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    SpriteRenderer[] Renderers;

    [SerializeField]
    Material DefaultMaterial;

    private void Awake()
    {
        Renderers = GetComponentsInChildren<SpriteRenderer>(true);

        List<SpriteRenderer> newRenderers = new List<SpriteRenderer>();//TODO Optimize the part from this point onward?

        foreach(SpriteRenderer renderer in Renderers)
        {
            if(renderer.material == DefaultMaterial)
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
        if(DefaultMaterial == null)
        {
            DefaultMaterial = CORE.Instance.Data.DefaultSpriteMaterial;
        }

        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.material = DefaultMaterial;
        }
    }
    
}
