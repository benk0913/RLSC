using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpriteEntity : MonoBehaviour
{
    [SerializeField]
    Sprite[] Varriety;

    [SerializeField]
    SpriteRenderer Renderer;

    [SerializeField]
    bool AdjustScale = true;

    [SerializeField]
    float targetHeight = 1f;

    private void OnEnable()
    {
        Renderer.sprite = Varriety[Random.Range(0, Varriety.Length)];

        if (AdjustScale)
        {
            Bounds bounds = Renderer.sprite.bounds;
            float factor = targetHeight / bounds.size.y;
            transform.localScale = new Vector3(factor, factor, factor);
        }
    }
}
