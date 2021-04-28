using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffectUI : MonoBehaviour
{
    [SerializeField]
    public string SoundEffect;

    public virtual void Show(object data)
    {
        if (!string.IsNullOrEmpty(SoundEffect))
        {
            AudioControl.Instance.Play(SoundEffect);
        }
    }
}
