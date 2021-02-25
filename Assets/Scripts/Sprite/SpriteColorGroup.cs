using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorGroup : MonoBehaviour
{
    [SerializeField]
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

    public void SetMaterial(Material mat)
    {
        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.material = mat;
        }
    }

    public void ResetMaterial()
    {
        SetMaterial(CORE.Instance.Data.DefaultSpriteMaterial);
    }
    
}
