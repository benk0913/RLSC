﻿using System.Collections;
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
            renderer.material = DefaultMaterial;
        }
    }
    
}
