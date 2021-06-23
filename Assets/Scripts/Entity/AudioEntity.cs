using EdgeworldBase;
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

    public void SetMusic(string musicKey)
    {
        AudioControl.Instance.SetMusic(musicKey);
    }

    public void SetMusicToSceneDefault()
    {
        AudioControl.Instance.SetMusic(CORE.Instance.ActiveSceneInfo.MusicTrack);
    }

    public void SetSoundscape(string soundscapeKey)
    {
        AudioControl.Instance.SetSoundscape(soundscapeKey);
    }

    public void SetSoundscapeToSceneDefault()
    {
        AudioControl.Instance.SetSoundscape(CORE.Instance.ActiveSceneInfo.Soundscape);
    }


}
