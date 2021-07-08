using System.Collections;
using System.Collections.Generic;
using EdgeworldBase;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour {

    [SerializeField]
    Slider MasterVolume;

    [SerializeField]
    Slider SFXVolume;

    [SerializeField]
    Slider MusicVolume;

    void Start()
    {
        CORE.Instance.ConditionalInvokation(X=>!ResourcesLoader.Instance.m_bLoading,()=>{Init();});
    }

    public void Init()
    {
        SFXVolume.value = AudioControl.Instance.GetVolumeByTag("Untagged");
        MusicVolume.value = AudioControl.Instance.GetVolumeByTag("Music");
        MasterVolume.value = AudioListener.volume;
    }

    public void RefreshVolumeSFX(string Tag)
    {
        AudioControl.Instance.SetVolume(Tag, SFXVolume.value);
    }

    public void RefreshVolumeMusic(string Tag)
    {
        AudioControl.Instance.SetVolume(Tag, MusicVolume.value);
    }

    public void RefreshVolumeMaster()
    {
        AudioControl.Instance.SetMasterVolume(MasterVolume.value);
    }
}
