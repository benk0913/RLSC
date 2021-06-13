using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEntity : MonoBehaviour
{
    string lastSoundscapeSet;

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
        if(!string.IsNullOrEmpty(lastSoundscapeSet))
        {
            AudioControl.Instance.StopSound(lastSoundscapeSet);
            lastSoundscapeSet = "";
        }

        if (!string.IsNullOrEmpty(CORE.Instance.ActiveSceneInfo.Soundscape))
        {
            AudioControl.Instance.StopSound(CORE.Instance.ActiveSceneInfo.Soundscape);
        }

        lastSoundscapeSet = soundscapeKey;
        AudioControl.Instance.Play(soundscapeKey,true);
    }

    public void SetSoundscapeSceneDefault()
    {
        if (!string.IsNullOrEmpty(lastSoundscapeSet))
        {
            AudioControl.Instance.StopSound(lastSoundscapeSet);
        }

        if(string.IsNullOrEmpty(CORE.Instance.ActiveSceneInfo.Soundscape))
        {
            return;
        }

        AudioControl.Instance.Play(CORE.Instance.ActiveSceneInfo.Soundscape, true);
    }
}
