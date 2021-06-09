using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEntity : MonoBehaviour
{
    public void PlaySound(string soundKey)
    {
        if(string.IsNullOrEmpty(soundKey))
        {
            return;
        }

        AudioControl.Instance.Play(soundKey);
    }

    public void Play3DSound(string soundKey)
    {
        if (string.IsNullOrEmpty(soundKey))
        {
            return;
        }

        AudioControl.Instance.PlayInPosition(soundKey, transform.position);
    }
}
