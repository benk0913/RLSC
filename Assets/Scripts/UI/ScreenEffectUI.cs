using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEffectUI : MonoBehaviour
{
    [SerializeField]
    public string SoundEffect;

    public bool IsFlash;

    public virtual void Show(object data)
    {
        if(IsFlash && !SettingsMenuUI.Instance.FlashShake)
        {
            this.gameObject.SetActive(false);
            return;
        }
        if (!string.IsNullOrEmpty(SoundEffect))
        {
            AudioControl.Instance.Play(SoundEffect);
        }
    }
}
