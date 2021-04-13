using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    List<SpriteColorGroupInstance> Renderers = new List<SpriteColorGroupInstance>();


    private void Start()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        Renderers.Clear();
        foreach (SpriteRenderer renderer in renderers)
        {
            Renderers.Add(new SpriteColorGroupInstance(renderer));
        }
        
    }

    public void SetColor(Color clr)
    {
        foreach(SpriteColorGroupInstance inst in Renderers)
        {
            inst.Renderer.color = clr;
        }
    }

    public void SetAlpha(float alpha)
    {
        foreach (SpriteColorGroupInstance inst in Renderers)
        {
            inst.Renderer.color = new Color(inst.OriginalColor.r, inst.OriginalColor.g, inst.OriginalColor.b,alpha);
        }
    }

    public void SetMaterial(Material mat)
    {
        foreach (SpriteColorGroupInstance inst in Renderers)
        {
            inst.Renderer.material = mat;
        }
    }

    public void ResetMaterial()
    {
        foreach (SpriteColorGroupInstance inst in Renderers)
        {
            inst.Renderer.material = inst.OriginalMaterial;
        }
    }

    public void ResetColor()
    {
        foreach (SpriteColorGroupInstance inst in Renderers)
        {
            inst.Renderer.color = inst.OriginalColor;
        }
    }


    public class SpriteColorGroupInstance
    {
        public SpriteRenderer Renderer;
        public Color OriginalColor;
        public Material OriginalMaterial;

        public SpriteColorGroupInstance(SpriteRenderer renderer)
        {
            this.Renderer = renderer;
            this.OriginalColor = renderer.color;
            this.OriginalMaterial = renderer.material;
        }
    }
}
